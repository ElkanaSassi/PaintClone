using Server.Logger;
using SharedModels.CommunicationModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Network.Messaging.RequestHandlers
{
    public class LoadCanvasHandler : RequestHandler
    {
        public MessageType Type { get { return MessageType.LoadCanvas; } }

        public async Task HandleAsync(RequestInfo request, NetworkStream stream, List<string> openFiles)
        {
            // TODO: needs to add a way to know when client stop loading one of the canvases. to "unlock" it...
            string fileName = Encoding.ASCII.GetString(request.Data);

            ResponseInfo response = LoadCanvasFromFile(fileName, openFiles);
            ServerLogger.Log(request.MessageType, request.From, response.IsSuccess);

            await ResponseBuilder.SendResponseToClientAsync(response, stream);
        }

        public ResponseInfo LoadCanvasFromFile(string fileName, List<string> openFiles)
        {
            // if file is lock = a different client is currently working on the same file.
            lock (openFiles)
            {
                if (openFiles.Contains(fileName))
                {
                    return new ResponseInfo { IsSuccess = false, Message = "ERROR: File is already open by another client." };
                }
                // if we got here,
                // the requested file is not in use.

                string completePath = Path.Combine(LocalModels.LocalModels.canvasDirectory, fileName);
                if (File.Exists(completePath))
                {
                    using (StreamReader Shapesfile = new StreamReader(completePath))
                    {
                        openFiles.Add(fileName);
                        var data = Encoding.UTF8.GetBytes(Shapesfile.ReadToEnd());
                        return new ResponseInfo { IsSuccess = true, Message = "File opened successfully.", Data = data };
                    }
                }
                else
                {
                    return new ResponseInfo { IsSuccess = false, Message = "ERROR: file is missing." };
                }

            }
        }
    }
}
