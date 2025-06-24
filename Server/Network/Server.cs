using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SharedModels.CommunicationModels;
using Server.Network.Messaging;
using Server.Network.Messaging.RequestHandlers;

namespace Server.Network
{
    public class Server
    {
        private readonly TcpListener _listener; 
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly List<TcpClient> _clients = new List<TcpClient>();
        private static List<string> _openFiles = new();

        private readonly Dictionary<MessageType, RequestHandler> _handlers = new();

        public Server(string ipAddress = "127.0.0.1", int port = 3035)
        {
            _listener = new TcpListener(IPAddress.Parse(ipAddress), port); // at the moment only localhost
            Console.WriteLine($"Server started on port {port}...");
            RegisterHandlers();
        }

        public async Task StartAsync()
        {
            _listener.Start();
            var token = _cts.Token; 

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var client = await _listener.AcceptTcpClientAsync(token);
                    Console.WriteLine("Client connected.");

                    lock (_clients)
                    {
                        _clients.Add(client);
                    }

                    _ = HandleClientAsync(client);
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Server stopping, cancellation requested.");
            }
            finally
            {
                _listener.Stop();
                Console.WriteLine("Server stopped.");
            }
        }

        public async Task StopAsync()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }

            lock (_clients)
            {
                foreach (var client in _clients)
                {
                    try
                    {
                        client.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error closing a client: {ex.Message}");
                    }
                }
                _clients.Clear();
            }
        }

        private void RegisterHandlers()
        {
            var handlers = new RequestHandler[]
            {
                new LoadCanvasHandler(),
                new UploadCanvasHandler(),
                new FileNameValidationHandler(),
                new GetStoredFilesHandler()
            };

            foreach (var handler in handlers)
            {
                _handlers[handler.Type] = handler;
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                await using NetworkStream stream = client.GetStream();

                while (client.Connected)
                {
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

                    RequestInfo requestInfo = JsonSerializer.Deserialize<RequestInfo>(messageBuffer);

                    if (_handlers.TryGetValue(requestInfo.MessageType, out var handler))
                    {
                        await handler.HandleAsync(requestInfo, stream, _openFiles);
                    }
                    else
                    {
                        var response = ResponseBuilder.UnknownRequest();
                        await ResponseBuilder.SendResponseToClientAsync(response, stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client disconnected: {ex.Message}");
            }
            finally
            {
                lock (_clients)
                {
                    _clients.Remove(client);
                    client.Close();
                }
            }
        }

    }
}