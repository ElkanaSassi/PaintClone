using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SharedModels;

namespace Server.Network
{
    public class ShapeServer
    {
        private readonly TcpListener _listener;
        private readonly List<TcpClient> _clients = new List<TcpClient>();
        private static List<string> _openFiles = new();

        public ShapeServer(string ipAddress = "127.0.0.1", int port = 3035)
        {
            _listener = new TcpListener(IPAddress.Parse(ipAddress), port); // at the moment only localhost
            Console.WriteLine($"Server started on port {port}...");
        }

        public async Task StartAsync()
        {
            _listener.Start();
            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected.");

                _clients.Add(client);

                // handle client in a new task to allow server to accept more clients
                _ = HandleClientAsync(client);
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                while (client.Connected)
                { // TODO: client massages handling.
                    await using NetworkStream stream = client.GetStream();
                    RequestInfo requestInfo = new RequestInfo();

                    /* client stream decomposed
                        first 4 bytes - the length of the massage in bytes.
                        after we have the length, we read from the stream exactly that many bytes. 
                     */

                    // geting massage length
                    byte[] lengthBuffer = new byte[4];
                    await stream.ReadExactlyAsync(lengthBuffer, 0, 4);
                    int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

                    // read exactly that many bytes
                    byte[] messageBuffer = new byte[messageLength];
                    await stream.ReadExactlyAsync(messageBuffer, 0, messageLength);

                    
                    requestInfo = JsonSerializer.Deserialize<RequestInfo>(messageBuffer);

                    await HandleRequestAsync(requestInfo, stream);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client disconnected: {ex.Message}");
            }
            finally
            {
                _clients.Remove(client);
                client.Close();
            }
        }

        private async Task HandleRequestAsync(RequestInfo requestInfo, NetworkStream clientStream)
        {
            switch(requestInfo.MessageType)
            {
                case MessageType.LoadCanvas:
                    {
                        // TODO: needs to add a way to know when client stop loading one of the canvases. to unlock it...
                        string fileName = Encoding.ASCII.GetString(requestInfo.Data);

                        ResponseInfo response = TryOpenFile(fileName);
                        ServerLogger.Log(requestInfo.MessageType ,requestInfo.From, response.IsSuccess);

                        await SendResponseToClientAsync(response, clientStream);
                        break;
                    }
                case MessageType.UploadCanvas:
                    {
                        // -the data strcture of Upload request-
                        // 0-4      -> (X) file name length.
                        // 4-X      -> file name.
                        // X-END    -> shapes in Json presentation. 
                        int fileNameLength = BitConverter.ToInt32(requestInfo.Data, 0); // 32bit - first 4 bytes.
                        string fileName = Encoding.ASCII.GetString(requestInfo.Data, 4, fileNameLength);
                        string shapeData = Encoding.ASCII.GetString(requestInfo.Data.Skip(4 + fileNameLength).ToArray());

                        ResponseInfo response = storeShapesInServer(fileName, shapeData);
                        ServerLogger.Log(requestInfo.MessageType, requestInfo.From, response.IsSuccess);

                        await SendResponseToClientAsync(response, clientStream);
                        break;
                    }
                case MessageType.FileNameValidation:
                    {
                        string fileName = Encoding.ASCII.GetString(requestInfo.Data);

                        ResponseInfo response = IsFileNameExists(fileName);
                        ServerLogger.Log(requestInfo.MessageType, requestInfo.From, response.IsSuccess);

                        await SendResponseToClientAsync(response, clientStream);
                        break;
                    }
                case MessageType.GetStoredFiles:
                    {
                        ResponseInfo response = new ResponseInfo();
                        response.IsSuccess = true;
                        response.Message = string.Join(",", getStroedFilesInServer());
                        ServerLogger.Log(requestInfo.MessageType, requestInfo.From, response.IsSuccess);

                        await SendResponseToClientAsync(response, clientStream);
                        break;
                    }
                default:
                    {
                        ResponseInfo response = new ResponseInfo { IsSuccess = false, Message = "ERROR: Unknown request!" };
                        _ = SendResponseToClientAsync(response, clientStream);
                        break;
                    }
            }
        }

        public ResponseInfo storeShapesInServer(string fileName, string shapes) 
        {
            try
            {
                // copmlete relative path
                string completePath = Path.Combine(LocalModels.LocalModels.canvasDirectory, fileName);

                using (FileStream Shapesfile = File.Create(completePath))
                {
                    Shapesfile.Write(Encoding.UTF8.GetBytes(shapes));
                }

                return new ResponseInfo { IsSuccess = true, Message = "Shapes were successfully stored!!!" };
            }
            catch (Exception ex)
            {
                return new ResponseInfo {IsSuccess = false, Message = "Error occurred in the file storage procedure!" };
            }

        }

        public List<string> getStroedFilesInServer()
        {
            string canvasDir = LocalModels.LocalModels.canvasDirectory;

            var files = Directory.GetFiles(canvasDir, "*.json");
            List<string> fileNames = files.Select(f => Path.GetFileName(f)).ToList();

            return fileNames;
        }

        public async Task SendResponseToClientAsync(ResponseInfo response, NetworkStream clientStream)
        {
            string JsonAsString = JsonSerializer.Serialize(response);
            byte[] JsonAsBytes = Encoding.UTF8.GetBytes(JsonAsString);

            int responedLength = JsonAsBytes.Length;
            byte[] lengthBytes = BitConverter.GetBytes(responedLength);

            await clientStream.WriteAsync(lengthBytes, 0, lengthBytes.Length);
            await clientStream.WriteAsync(JsonAsBytes, 0, JsonAsBytes.Length);
        }

        public ResponseInfo TryOpenFile(string fileName)
        {
            lock (_openFiles)
            {
                if (_openFiles.Contains(fileName))
                {
                    return new ResponseInfo { IsSuccess = false, Message = "ERROR: File is already open by another client."};
                }
                // if we got here,
                // the requested file is not in loaded.
                _openFiles.Add(fileName);
                return new ResponseInfo { IsSuccess = true, Message = "File opened successfully." };
            }
        }

        public ResponseInfo IsFileNameExists(string fileName)
        {
            string canvasDir = LocalModels.LocalModels.canvasDirectory;

            if (Directory.Exists(canvasDir))
            {
                var files = Directory.GetFiles(canvasDir, "*.json");

                files.Select(f => System.IO.Path.GetFileName(f));

                if(files.Contains(fileName))
                {
                    return new ResponseInfo { IsSuccess = false, Message = "ERROR: File name allready exists, choose another." };
                }
            }

            // if we got here,
            // the requested file name is not in use.
            return new ResponseInfo { IsSuccess = true, Message = "The choosen name is valid"};
        }

    }
}