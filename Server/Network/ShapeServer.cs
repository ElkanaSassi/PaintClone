using Client.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.Network
{
    public class ShapeServer
    {
        private readonly TcpListener _listener;
        private readonly List<TcpClient> _clients = new List<TcpClient>();

        public ShapeServer(string ipAddress = "127.0.0.1", int port = 1008)
        {
            _listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            Console.WriteLine($"Server started on port {port}...");
        }

        public async Task StartAsync()
        {
            _listener.Start();
            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected.");


                // handle client in a new task to allow server to accept more clients
                _ = HandleClientAsync(client);
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                while (client.Connected)
                {
                    // TODO: client massages handling.
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

    }
}