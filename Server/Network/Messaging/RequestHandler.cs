using SharedModels.CommunicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Network.Messaging
{
    public interface RequestHandler
    {
        MessageType Type { get; }
        Task HandleAsync(RequestInfo request, NetworkStream stream, List<string> openFiles);
    }
}
