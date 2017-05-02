namespace AMQPOverWebSocketProxy.WebSocket.Messages
{
    public class FormatErrorMessage
    {
        public FormatErrorMessage(string error)
        {
            Error = error;
        }

        public string Key { get; } = "FormatErrorMessage";

        public string Error { get; }
    }
}