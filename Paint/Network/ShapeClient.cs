using Client.Services;
using SharedModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
namespace Client.Network
{
    public class ShapeClient
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private Canvas _canvas;

        public ShapeClient(Canvas canvas, string serverIp = "127.0.0.1", int port = 3035)
        {
            _canvas = canvas;
            _client = new TcpClient();
            _client.Connect(serverIp, port);
            _stream = _client.GetStream();
        }

        public async Task SendShapesAsync(List<ShapeData> shapes, string fileName)
        {
            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
            byte[] fileNameLengthBytes = BitConverter.GetBytes(fileNameBytes.Length);

            string shapeJson = JsonSerializer.Serialize(shapes);
            byte[] shapeJsonBytes = Encoding.UTF8.GetBytes(shapeJson);

            // build the complete message. [file name length + file name + shape json]
            byte[] completeMessage = new byte[4 + fileNameBytes.Length + shapeJsonBytes.Length];
            Buffer.BlockCopy(fileNameLengthBytes, 0, completeMessage, 0, 4); // file name length
            Buffer.BlockCopy(fileNameBytes, 0, completeMessage, 4, fileNameBytes.Length); // file name in bytes
            Buffer.BlockCopy(shapeJsonBytes, 0, completeMessage, 4 + fileNameBytes.Length, shapeJsonBytes.Length); // shape json in bytes

            RequestInfo request = new RequestInfo();
            request.MessageType = MessageType.UploadCanvas;
            request.From = _client.Client.RemoteEndPoint.ToString();
            request.Data = completeMessage;

            _ = SendRequestToServerAsync(request);

        } 
        
        public async Task SendRequestToServerAsync(RequestInfo request)
        {
            string JsonAsString = JsonSerializer.Serialize(request);
            byte[] JsonAsBytes = Encoding.UTF8.GetBytes(JsonAsString);

            int responedLength = JsonAsBytes.Length;
            byte[] lengthBytes = BitConverter.GetBytes(responedLength);

            await _stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);
            await _stream.WriteAsync(JsonAsBytes, 0, JsonAsBytes.Length);    
        }

        public async Task<ResponseInfo> GetResponseFromServer()
        {
            // geting massage length
            byte[] lengthBuffer = new byte[4];
            await _stream.ReadExactlyAsync(lengthBuffer, 0, 4);
            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

            // read exactly that many bytes
            byte[] messageBuffer = new byte[messageLength];
            await _stream.ReadExactlyAsync(messageBuffer, 0, messageLength);

            ResponseInfo responseInfo = JsonSerializer.Deserialize<ResponseInfo>(messageBuffer);

            return responseInfo;
        }

        public async Task<List<string>> GetStoredFilesInServer()
        {
            RequestInfo request = new RequestInfo();
            request.MessageType = MessageType.GetStoredFiles;
            request.From = _client.Client.RemoteEndPoint.ToString();
            request.Data = null;

            _ = SendRequestToServerAsync(request);

            ResponseInfo response = await GetResponseFromServer();

            List<string> fileNames = response.Message.Split(',').ToList();

            return fileNames;
        }

        public async Task sendFlieRequestAsync(string fileName)
        {
            RequestInfo request = new RequestInfo();
            request.MessageType = MessageType.LoadCanvas;
            request.From = _client.Client.RemoteEndPoint.ToString();
            request.Data = Encoding.UTF8.GetBytes(fileName);

            _ = SendRequestToServerAsync(request);
        }

        public async Task ReceiveShapesAsync()
        {
            // geting massage length
            byte[] lengthBuffer = new byte[4];
            await _stream.ReadExactlyAsync(lengthBuffer, 0, 4);
            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

            // read exactly that many bytes
            byte[] messageBuffer = new byte[messageLength];
            await _stream.ReadExactlyAsync(messageBuffer, 0, messageLength);
            string messageAsString = Encoding.UTF8.GetString(messageBuffer);

            ResponseInfo response = JsonSerializer.Deserialize<ResponseInfo>(messageAsString);

            List<ShapeData> shapes = JsonSerializer.Deserialize<List<ShapeData>>(response.Data);

            ShapeSerializer.LoadFromJson(_canvas, shapes);
        }

        public async Task<bool> FileNameValidation(string fileName)
        {
            RequestInfo request = new RequestInfo();

            request.MessageType = MessageType.FileNameValidation;
            request.From = _client.Client.RemoteEndPoint.ToString(); // on local host, it be the same. (uniqueness for the ip and port only..)
            request.Data = Encoding.UTF8.GetBytes(fileName);

            _ = SendRequestToServerAsync(request);

            // geting massage length
            byte[] lengthBuffer = new byte[4];
            await _stream.ReadExactlyAsync(lengthBuffer, 0, 4);
            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

            // read exactly that many bytes
            byte[] messageBuffer = new byte[messageLength];
            await _stream.ReadExactlyAsync(messageBuffer, 0, messageLength);

            ResponseInfo response = JsonSerializer.Deserialize<ResponseInfo>(messageBuffer);

            return response.IsSuccess;
        }

        public void Close()
        {
            _stream?.Close(); // if not null
            _client?.Close();
        }
    }
}

