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
    public class FileNameValidationHandler : RequestHandler
    {
        public MessageType Type { get { return MessageType.FileNameValidation; } }

        public async Task HandleAsync(RequestInfo request, NetworkStream stream, List<string> openFiles)
        {
            string fileName = Encoding.ASCII.GetString(request.Data);

            ResponseInfo response = IsFileNameExists(fileName);
            ServerLogger.Log(request.MessageType, request.From, response.IsSuccess);

            await ResponseBuilder.SendResponseToClientAsync(response, stream);
        }

        public ResponseInfo IsFileNameExists(string fileName)
        {
            string canvasDir = LocalModels.LocalModels.canvasDirectory;

            if (Directory.Exists(canvasDir))
            {
                var files = Directory.GetFiles(canvasDir, "*.json");

                files.Select(f => System.IO.Path.GetFileName(f));

                if (files.Contains(fileName))
                {
                    return new ResponseInfo { IsSuccess = false, Message = "ERROR: File name allready exists, choose another." };
                }
            }

            // if we got here,
            // the requested file name is not in use.
            return new ResponseInfo { IsSuccess = true, Message = "The choosen name is valid" };
        }
    }
}
