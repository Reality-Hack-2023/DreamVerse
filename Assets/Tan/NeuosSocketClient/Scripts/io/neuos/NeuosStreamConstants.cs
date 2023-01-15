namespace io.neuos
{
    public static class NeuosStreamConstants
    {

        /**
         * General Constants
         * */
        // Name of broadcasting service on the local network
        public const string SERVICE_NAME = "NeuosService";
        // Service type / protocol for network discovery
        public const string SERVICE_TYPE = "_http._tcp";

        /**
         * List of constants that can be sent / received as keys inside JSON objects over the
         * socket connection
         * */
        public static class StreamObjectKeys {
            // defines the command that is encapsulated in this JSON
            public const string COMMAND = "command";
            // defines the times stamp in unix ms that the data has arrived from the prediction service
            // value is a long int
            public const string TIME_STAMP = "timestamp";
            // key for previous connection state ( when COMMAND == CONNECTION)
            public const string PREVIOUS = "previous";
            // key for current connection state ( when COMMAND == CONNECTION)
            public const string CURRENT = "current";
            // key that holds the key name of the stream value ( when COMMAND == VALUE_CHANGED )
            public const string KEY = "key";
            // key that holds the new value ( when COMMAND == VALUE_CHANGED )
            public const string VALUE = "value";
            // key that holds the challenge request / response ( when COMMAND == CHALLENGE )
            public const string CHALLENGE_DATA = "challenge_data";
            // key to use when sending apiKey to server ( when COMMAND == AUTH )
            public const string API_KEY = "apiKey";
            // key that holds the passed value of QA data ( when COMMAND == QA )
            public const string PASSED = "passed";
            // key that holds the type of QA failure if PASSED == false ( when COMMAND == QA )
            public const string TYPE = "type";
            // key that holds the error code sent by an error command ( when COMMAND == ERROR )
            public const string ERROR_CODE = "errorCode";
            // key that holds the error message sent by an error command ( when COMMAND == ERROR )
            public const string MESSAGE = "message";
        }

        /**
         * Possible values that a COMMAND key can hold in a transported object
         * */
        public static class StreamCommandValues {
            // object describes a connection change
            public const string CONNECTION = "connection";
            // object describes a value change
            public const string VALUE_CHANGED = "valueChange";
            // object describes QA status
            public const string QA = "qa";
            // object notifies that the session has completed
            public const string SESSION_COMPLETE = "sessionComplete";
            // object describes an error
            public const string ERROR = "error";
            // object for requesting authentication with server
            public const string AUTH = "auth";
            // object contains auth challenge / response
            public const string CHALLENGE = "challenge";
            // object notifies successful authentication
            public const string AUTH_SUCCESS = "auth-success";
            // object notifies failed authentication
            public const string AUTH_FAILED = "auth-failed";
        }

        /**
         * Names of prediction values available 
         */
        public static class PredictionValues
        {
            public const string ZONE_STATE = "zone_state";
            public const string FOCUS_STATE = "focus";
            public const string ENJOYMENT_STATE = "enjoyment";
            public const string HEART_RATE = "heart_rate";
            public const string AVG_MOTION = "avg_motion";
        }

        /**
         * QA Failure type results
         * */
        public static class QAFailureType
        {
            public const int HEADBAND_OFF_HEAD = 1;
            public const int MOTION_TOO_HIGH = 2;
            public const int EEG_FAILURE = 3;
        }

        /**
        * Connection values.
        **/
        public static class ConnectionState
        {
            public const int UNKNOWN = 0;
            public const int CONNECTING = 1;
            public const int CONNECTED = 2;
            public const int CONNECTION_FAILED = 3;
            public const int DISCONNECTED = 4;
            public const int DISCONNECTED_UPON_REQUEST = 5;
        }

    }
}