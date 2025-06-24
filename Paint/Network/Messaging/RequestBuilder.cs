using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using SharedModels.CommunicationModels;
using SharedModels.Shapes;

namespace Client.Network.Messaging
{
    public static class RequestBuilder
    {
        public static RequestInfo BuildUploadCanvasRequest(List<Shape> shapes, string fileName)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(shapes, options);
            var fileNameBytes = Encoding.UTF8.GetBytes(fileName);
            var jsonBytes = Encoding.UTF8.GetBytes(json);

            // [fileNameLength(4) | fileName | json]
            var data = new byte[4 + fileNameBytes.Length + jsonBytes.Length];
            Buffer.BlockCopy(BitConverter.GetBytes(fileNameBytes.Length), 0, data, 0, 4);
            Buffer.BlockCopy(fileNameBytes, 0, data, 4, fileNameBytes.Length);
            Buffer.BlockCopy(jsonBytes, 0, data, 4 + fileNameBytes.Length, jsonBytes.Length);

            return new RequestInfo {MessageType = MessageType.UploadCanvas, Data = data};
        }

        public static RequestInfo BuildGetStoredFilesRequest()
        {
            return new RequestInfo {MessageType = MessageType.GetStoredFiles, Data = null};
        }

        public static RequestInfo BuildLoadCanvasRequest(string fileName)
        {
            return new RequestInfo {MessageType = MessageType.LoadCanvas, Data = Encoding.UTF8.GetBytes(fileName)};
        }

        public static RequestInfo BuildFileNameValidationRequest(string fileName)
        { 
            return new RequestInfo {MessageType = MessageType.FileNameValidation, Data = Encoding.UTF8.GetBytes(fileName)};
        }
    }
}
