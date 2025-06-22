namespace SharedModels
{
    public enum MessageType
    {
        LoadCanvas,
        UploadCanvas,
        FileNameValidation,
        GetStoredFiles
    }

    public class RequestInfo
    {
        public MessageType MessageType { get; set; }
        public string From { get; set; }
        public byte[] Data { get; set; }
    }

    public class ResponseInfo
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public byte[] Data { get; set; }
    }
}