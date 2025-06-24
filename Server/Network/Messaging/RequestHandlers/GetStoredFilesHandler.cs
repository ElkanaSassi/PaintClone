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
    public class GetStoredFilesHandler : RequestHandler
    {
        public MessageType Type { get { return MessageType.GetStoredFiles; } }

        public async Task HandleAsync(RequestInfo request, NetworkStream stream, List<string> openFiles)
        {
            ResponseInfo response = new ResponseInfo();
            response.IsSuccess = true;
            response.Message = string.Join(",", GetStroedFilesInServer());
            ServerLogger.Log(request.MessageType, request.From, response.IsSuccess);

            await ResponseBuilder.SendResponseToClientAsync(response, stream);
        }

        public List<string> GetStroedFilesInServer()
        {
            string canvasDir = LocalModels.LocalModels.canvasDirectory;

            var files = Directory.GetFiles(canvasDir, "*.json");
            List<string> fileNames = files.Select(f => Path.GetFileName(f)).ToList();

            return fileNames;
        }
    }
}
