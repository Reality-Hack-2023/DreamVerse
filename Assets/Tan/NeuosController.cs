using io.neuos;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Game controller behaviour
/// This class connectes between the "game" and the neuos client
/// Though a very basic implementation it demonstrates how to activate and use the 
/// NeuosClient and the information is provides.
/// This class recieves data from the NeuosClient via its serialized events
/// which are linked in the scene with the inspector.
/// See the Neuos Client / Game Controller GameObjecs in the Demo scene
/// </summary>
public class NeuosController : MonoBehaviour
{
    [SerializeField]
    private string ip = "192.168.183.104";
    private const int port = 38641;
    private const string ApiKey = "hnracoWcoq3vNTg2J";
    [SerializeField]
    private NeuosStreamClient neuosStreamClient;
    
    
    StringBuilder builder = new StringBuilder();
    StringBuilder arrayBuilder = new StringBuilder();
    public static Dictionary<string, string> fields = new Dictionary<string, string>();
    /// <summary>
    /// Method to call that will connect to the Neuos Stream server
    /// </summary>
    public void ConnectToServer()
    {
        if (!neuosStreamClient.IsConnected)
        {
            neuosStreamClient.ApiKey = ApiKey;
            Debug.Log("Connecting");
            neuosStreamClient.ConnectToServer(ip, port);
        }
    }
    /// <summary>
    /// Method to call that will disconnect from the Neuos Stream server
    /// </summary>
    public void DisconnectFromServer()
    {
        if (neuosStreamClient.IsConnected)
        {
            neuosStreamClient.Disconnect();
        }
    }
    /// <summary>
    /// Callback for when the Neuos Stream server has connected
    /// </summary>
    public void OnServerConnected()
    {
        Debug.Log("Connected to Neuos "+ ip + " on port " + port);

    }
    /// <summary>
    /// Callback for when the Neuos Stream server has disconnected
    /// </summary>
    public void OnServerDisconnected()
    {
        Debug.Log("Disconnected from Neuos " + ip + " on port " + port);
    }
    /// <summary>
    /// Callback for when the Neuos Stream server sends an updated value
    /// </summary>
    /// <param name="key">The key of the value</param>
    /// <param name="value">The actual value</param>
    public void OnValueChanged(string key, float value)
    {
        // here we store the value into our dictionary
        fields[key] = value.ToString();
    }
    /// <summary>
    /// Callback for when the Neuos Stream server sends an updated value
    /// </summary>
    /// <param name="key">The key of the value</param>
    /// <param name="value">The actual value</param>
    public void OnArrayValueChanged(string key, float[] value)
    {
        // here we store the value into our dictionary
        arrayBuilder.Clear();
        foreach (var kvp in value)
        {
            // add each key value pair as a line to the string builder
            arrayBuilder.Append($"{kvp},");
        }
        // update the UI text value with the value of the new string builder
        arrayBuilder.Length--;
        fields[key] = arrayBuilder.ToString();
    }
    /// <summary>
    /// Callback for when the connection status of the headband changes
    /// </summary>
    /// <param name="prev"></param>
    /// <param name="curr"></param>
    public void OnHeadbandConnectionChange(int prev, int curr)
    {
        // values defined in NeuosStreamConstants.ConnectionState
        fields["HeadbandConnection"] = $"Current : {curr} Previous : {prev}";
        Debug.Log(fields["HeadbandConnection"]);
    }
    /// <summary>
    /// Callback for when the Neuos Stream server updates its QA model
    /// </summary>
    /// <param name="passed">Did QA test pass</param>
    /// <param name="reason">If failed, what was the reason for failure</param>
    public void OnQAMessage(bool passed, int reason)
    {
        // reasons defined in NeuosStreamConstants.QAFailureType
        fields["QA"] = $"Passed : {passed} Reason : {reason}";
        // Debug.Log(fields["QA"]);
    }
    /// <summary>
    /// Called whenever the Neuos Stream server reports an error
    /// </summary>
    /// <param name="message">The error message</param>
    public void OnError(string message)
    {
        fields["Last error"] = message;
        Debug.Log(fields["Last error"]);
    }

    public static string GetField(string key) {
        return fields[key];
    }

    public static Dictionary<string, string> GetFields() {
        return fields;
    }

    public void Start() {
        ConnectToServer();
    }
}

