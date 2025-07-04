﻿using Client.Services;
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

        public ShapeClient(Canvas canvas, string serverIp = "127.0.0.1", int port = 1008)
        {
            _canvas = canvas;
            _client = new TcpClient();
            _client.Connect(serverIp, port);
            _stream = _client.GetStream();
        }

        public async Task SendShapesAsync(List<ShapeData> shapes)
        {
            var json = JsonSerializer.Serialize(shapes);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            byte[] lengthPrefix = BitConverter.GetBytes(jsonBytes.Length);

            await _stream.WriteAsync(lengthPrefix, 0, 4);
            await _stream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
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
            byte[] lengthBuffer = new byte[4];
            await _stream.ReadExactlyAsync(lengthBuffer, 0, 4);
            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

            // read exactly that many bytes
            byte[] messageBuffer = new byte[messageLength];
            await _stream.ReadExactlyAsync(messageBuffer, 0, messageLength);
            string messageAsString = Encoding.UTF8.GetString(messageBuffer);

            List<ShapeData> shapes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ShapeData>>(messageAsString);

            ShapeSerializer.LoadFromJson(_canvas, shapes);
        }

        public List<string> GetStoredFilesInServer()
        {
            RequestInfo request = new RequestInfo();
            request.MessageType = MessageType.GetStoredFiles;
            request.From = _client.Client.RemoteEndPoint.ToString();
            request.Data = null;

            _ = SendRequestToServerAsync(request);

            // geting massage length
            byte[] lengthBuffer = new byte[4];
            _stream.ReadExactlyAsync(lengthBuffer, 0, 4);
            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

            // read exactly that many bytes
            byte[] messageBuffer = new byte[messageLength];
            _stream.ReadExactlyAsync(messageBuffer, 0, messageLength);

            return null;
        }

        public bool FileNameValidation(string fileName)
        {
            RequestInfo request = new RequestInfo();

            request.MessageType = MessageType.FileNameValidation;
            request.From = _client.Client.RemoteEndPoint.ToString(); // on local host, it be the same. (uniqueness for the ip and port only..)
            request.Data = Encoding.UTF8.GetBytes(fileName);

            _ = SendRequestToServerAsync(request);

            // geting massage length
            byte[] lengthBuffer = new byte[4];
            _stream.ReadExactlyAsync(lengthBuffer, 0, 4);
            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

            // read exactly that many bytes
            byte[] messageBuffer = new byte[messageLength];
            _stream.ReadExactlyAsync(messageBuffer, 0, messageLength);

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

