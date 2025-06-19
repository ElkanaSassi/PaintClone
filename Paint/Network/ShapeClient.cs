using SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Network
{
    internal class ShapeClient
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;

        public ShapeClient(string serverIp = "127.0.0.1", int port = 1008)
        {
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

        public async Task SendToServerAsync(RequestInfo request )
        {

        }

        public async Task<List<ShapeData>> ReceiveShapesAsync()
        {
            // TO DO
            return null;
        }

        public void Close()
        {
            _stream?.Close(); // if not null
            _client?.Close();
        }
    }
}

