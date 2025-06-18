namespace SharedModels
{
    public class RequestInfo
    {
        public string MessageType { get; set; }
        public string From { get; set; }
        public byte[] Data { get; set; }
    }
}