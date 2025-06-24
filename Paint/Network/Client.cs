using Client.Network.Messaging;
using Client.Services;
using SharedModels.CommunicationModels;
using SharedModels.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using System.Windows.Controls;
namespace Client.Network
{
    public class Client
    {
        private readonly Stream _stream;

        public Client(string serverIp = "127.0.0.1", int port = 3035)
        {
            try
            {
                var tcp = new TcpClient();
                tcp.Connect(serverIp, port);
                _stream = tcp.GetStream();
            }
            catch(Exception ex) 
            { 
                Console.WriteLine(ex.Message);
            } 
        }

        public async Task<ResponseInfo> SendShapesAsync(List<Shape> shapes, string fileName)
        {
            var request = RequestBuilder.BuildUploadCanvasRequest(shapes, fileName);
            await StreamHelper.SendMessageAsync(_stream, request);

            var response = await ResponseHandler.ReadResponseAsync(_stream);
            return response;
        }

        public async Task<List<string>> GetStoredFilesAsync()
        {
            var request = RequestBuilder.BuildGetStoredFilesRequest();
            await StreamHelper.SendMessageAsync(_stream, request);

            var response = await ResponseHandler.ReadResponseAsync(_stream);
            return response.Message.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public Task RequestCanvasLoadAsync(string fileName)
        {
            var request = RequestBuilder.BuildLoadCanvasRequest(fileName);
            return StreamHelper.SendMessageAsync(_stream, request);
        }

        public async Task ReceiveAndRenderCanvasAsync(Canvas canvas)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var response = await ResponseHandler.ReadResponseAsync(_stream);
            string json = Encoding.UTF8.GetString(response.Data);
            var shapes = JsonSerializer.Deserialize<List<Shape>>(json);
            ShapeService.LoadShapesToCanvas(canvas, shapes);
        }

        public async Task<bool> ValidateFileNameAsync(string fileName)
        {
            var request = RequestBuilder.BuildFileNameValidationRequest(fileName);
            await StreamHelper.SendMessageAsync(_stream, request);

            var response = await ResponseHandler.ReadResponseAsync(_stream);
            return response.IsSuccess;
        }

        public void Close()
        {
            _stream?.Close();
        }
    }
}

