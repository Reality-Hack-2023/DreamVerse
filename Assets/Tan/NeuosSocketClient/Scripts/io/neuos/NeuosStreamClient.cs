using UnityEngine;
using System.Net.Sockets;
using System;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using UnityEngine.Events;
using System.Collections.Generic;
using static io.neuos.NeuosStreamConstants;

namespace io.neuos
{
    /// <summary>
    /// Stream Client Behaviour
    /// This class handles the connection to the Neuos Stream Server
    /// running on an android phone in the local network
    /// It establises the socket connection, and handles authentication, along with all messaging.
    /// The class also handles reading the JSON objects and dispatching events into the unity world.
    /// This is done via UnityEvents so it is possible to link in the inspector.
    /// </summary>
    public class NeuosStreamClient : MonoBehaviour
    {
        /**
         Serializable event classes used
         */
        [Serializable]
        public class ValueChangedEvent : UnityEvent<string, float> { }
        [Serializable]
        public class ArrayValueChangedEvent : UnityEvent<string, float[]> { }
        [Serializable]
        public class QAEvent : UnityEvent<bool, int> { }
        [Serializable]
        public class ErrorEvent : UnityEvent<string> { }
        [Serializable]
        public class ConnectionEvent : UnityEvent<int, int> { }

        /**
         Events sent out into Unity
         */
        [SerializeField]
        private UnityEvent OnServerConnected;
        [SerializeField]
        private UnityEvent OnServerDisconnected;
        [SerializeField]
        private ValueChangedEvent OnValueChanged;
        [SerializeField]
        private ArrayValueChangedEvent OnArrayValueChanged;
        [SerializeField]
        private ConnectionEvent OnHeadbandConnectionChanged;
        [SerializeField]
        private QAEvent OnQAEvent;
        [SerializeField]
        private ErrorEvent OnError;

        public bool IsConnected { get; private set; }
        // You must set this field with your API key before connecting
        public string ApiKey { get; set; }

        private Socket m_Socket;
        private byte[] m_CommandLength = new byte[2];
        private byte[] m_recBuffer = new byte[1024];
        private byte[] m_keepAlive = new byte[1];
        private bool m_blockingState;


        private JObject prevObject;

        /// <summary>
        /// Establishes a socket connection to the server
        /// API Key must be set prior or authentication will fail
        /// </summary>
        /// <param name="serverIp">IP address of the server</param>
        /// <param name="serverPort">Port number of the server</param>
        public void ConnectToServer(string serverIp, int serverPort)
        {
            try
            {
                // constract the IP / Port adderss
                IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
                // create a new TCPSocket with that address
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // attempt connection with the socket
                m_Socket.Connect(serverAddress);
#if UNITY_EDITOR
                Debug.Log($"Connections Status: {m_Socket.Connected}");
#endif
                // Start authentication process
                SendAuth();
            }
            catch (FormatException e)
            {
                OnError?.Invoke(e.Message);
#if UNITY_EDITOR
                Debug.LogError(e);
#endif
            }
            catch (SocketException ex)
            {
                OnError?.Invoke(ex.Message);
#if UNITY_EDITOR
                Debug.LogError(ex);
#endif
            }
        }
        /// <summary>
        /// Disconnects from the server and releases resources
        /// </summary>
        public void Disconnect()
        {
            IsConnected = false;
            try
            {
                m_Socket?.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
#if UNITY_EDITOR
                Debug.Log(e.Message);
#endif
            }
            finally
            {
                m_Socket?.Close();
            }
            m_Socket?.Dispose();
            OnServerDisconnected?.Invoke();
        }
        /// <summary>
        /// Unity update loop.
        /// Here the client reads 1 message off the stream every frame
        /// It then parses it and dispatches the data based using the class' events
        /// </summary>
        private void Update()
        {
            // only do work when connected
            if (IsConnected)
            {
                // the stream has 2 bytes written into it to denote the size of the message.
                // so we await until we have at least 2 bytes before reading the stream
                if (m_Socket.Available > 2)
                {
                    // get the next message
                    var data = GetMessage();
                    JObject response = null;
                    try
                    {
                        // parse the json into a JObject so we can pull properties out easily
                        response = JObject.Parse(data);
                        // get the command that arrived
                    }
                    catch(JsonReaderException e)
                    {
                        Debug.Log("Error: " + e);
                    }

                    if (response == null)
                    {

                        response = prevObject;

                    }
                    else
                    {
                        prevObject = response;
                    }

                        var commandValue = (string)response.Property(StreamObjectKeys.COMMAND)?.Value;
                        // example of pulling the time stamp off the command
                        var timestamp = (long)response.Property(StreamObjectKeys.TIME_STAMP)?.Value;



                        switch (commandValue)
                        {
                            case StreamCommandValues.VALUE_CHANGED:
                                {
                                    var key = (string)response.Property(StreamObjectKeys.KEY)?.Value;
                                    var type = response.Property(StreamObjectKeys.VALUE)?.Value.Type;
                                    if (type != JTokenType.Array)
                                    {
                                        var value = (float)response.Property(StreamObjectKeys.VALUE)?.Value;
                                        OnValueChanged?.Invoke(key, value);
                                    }
                                    else
                                    {
                                        var value = response.Property(StreamObjectKeys.VALUE)?.Value;
                                        List<float> floats = new List<float>();
                                        foreach (var child in value.Children())
                                        {
                                            floats.Add((float)child);
                                        }
                                        OnArrayValueChanged?.Invoke(key, floats.ToArray());
                                    }
                                    break;
                                }
                            case StreamCommandValues.QA:
                                {
                                    var passed = (bool)response.Property(StreamObjectKeys.PASSED)?.Value;
                                    var failure = (int)response.Property(StreamObjectKeys.TYPE)?.Value;
                                    OnQAEvent?.Invoke(passed, failure);
                                    break;
                                }
                            case StreamCommandValues.SESSION_COMPLETE:
                                {
                                    Disconnect();
                                    break;
                                }
                            case StreamCommandValues.CONNECTION:
                                {
                                    var previous = (int)response.Property(StreamObjectKeys.PREVIOUS)?.Value;
                                    var current = (int)response.Property(StreamObjectKeys.CURRENT)?.Value;
                                    OnHeadbandConnectionChanged?.Invoke(previous, current);
                                    break;
                                }
                        }
                }
                else // no data is avalable atm , so we verify our socket is still connected
                {

                    // This is how you can determine whether a socket is still connected.
                    m_blockingState = m_Socket.Blocking;
                    try
                    {
                        m_Socket.Blocking = false;
                        m_Socket.Send(m_keepAlive, 1, 0);
                    }
                    catch (SocketException e)
                    {
                        // 10035 == WSAEWOULDBLOCK
                        if (!e.NativeErrorCode.Equals(10035))
                        {
#if UNITY_EDITOR
                            Debug.Log("Connection lost");
#endif
                            Disconnect();
                        }
                    }
                    finally
                    {
                        if (IsConnected)
                        {
                            m_Socket.Blocking = m_blockingState;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Helper function that sends out and authentication command to the server
        /// </summary>
        private void SendAuth()
        {
            string toSend = GetAuth();
            // get the size of the message we are about to send
            ushort toSendLen = (ushort)Encoding.UTF8.GetByteCount(toSend);
            // turn it into bytes
            byte[] toSendBytes = Encoding.UTF8.GetBytes(toSend);
            // encode the size into bytes
            byte[] toSendLenBytes = BitConverter.GetBytes(toSendLen);
            // the server works as BigEndian. if we are running on little endian, we have to reverse the byte order
            if (BitConverter.IsLittleEndian)
                Array.Reverse(toSendLenBytes);
            // send message size
            m_Socket.Send(toSendLenBytes);
            // send message
            m_Socket.Send(toSendBytes);
#if UNITY_EDITOR
            Debug.Log("Sent Auth");
#endif
            // Wait for the server to respond
            GetAuthResponse();
        }
        /// <summary>
        /// Helper function to retrieve the server's authentican response
        /// </summary>
        private void GetAuthResponse()
        {
            // Get the message off the server
            var msg = GetMessage();
            // parse the json into a JObject so we can pull properties out easily
            var response = JObject.Parse(msg);
            // get the command that has arrived, should be the authentication response
            var commandValue = ((string)response.Property(StreamObjectKeys.COMMAND)?.Value);
            if (commandValue == StreamCommandValues.AUTH_SUCCESS)
            {
#if UNITY_EDITOR
                Debug.Log("Auth success");
#endif
                IsConnected = true;
                OnServerConnected?.Invoke();
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("Auth failed");
#endif
                OnError?.Invoke("Failed to authenticate with server");
            }
        }
        /// <summary>
        /// Helper function that gets the next message from the stream and return is parsed to a string
        /// </summary>
        /// <returns></returns>
        private string GetMessage()
        {
            // first get the 2 bytes to see the next message's size
            m_Socket.Receive(m_CommandLength);

            byte[] backupLength = m_CommandLength;
            // the server works as BigEndian. if we are running on little endian, we have to reverse the byte order
            if (BitConverter.IsLittleEndian)
            {
                Debug.Log("Reversed array");
                Array.Reverse(m_CommandLength);
            }
            
            // convert to a unsigned 16 bit int to find the length of the command
            int rcvLen = BitConverter.ToUInt16(m_CommandLength, 0);
            // read that length in byte into the buffer
            if(rcvLen > 1024)
            {
                rcvLen = 1023;
            }
            Debug.Log("Size: " + rcvLen);

            m_Socket.Receive(m_recBuffer, rcvLen, SocketFlags.None);
            // convert to a string
            string rcv = Encoding.UTF8.GetString(m_recBuffer, 0, rcvLen);
#if UNITY_EDITOR
            Debug.Log("Client received: " + rcv);
#endif
            return rcv;
        }
        /// <summary>
        /// Helper function to construct the authentication JSON object to send out to the server 
        /// </summary>
        /// <returns></returns>
        private string GetAuth()
        {
            var JObject = new JObject();
            JObject.Add(new JProperty(StreamObjectKeys.COMMAND, StreamCommandValues.AUTH));
            JObject.Add(new JProperty(StreamObjectKeys.API_KEY, ApiKey));
            return JObject.ToString();
        }
        /// <summary>
        /// Unity clean up function
        /// </summary>
        private void OnDestroy()
        {
            if (IsConnected)
            {
                IsConnected = false;
                m_Socket?.Close();
                m_Socket?.Dispose();
            }
        }
    }
}