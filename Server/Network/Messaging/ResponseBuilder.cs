using SharedModels.CommunicationModels;
using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.Network.Messaging
{
    public static class ResponseBuilder
    {
        public static async Task SendResponseToClientAsync(ResponseInfo response, NetworkStream clientStream)
        {
            string JsonAsString = JsonSerializer.Serialize(response);
            byte[] JsonAsBytes = Encoding.UTF8.GetBytes(JsonAsString);

            int responedLength = JsonAsBytes.Length;
            byte[] lengthBytes = BitConverter.GetBytes(responedLength);

            await clientStream.WriteAsync(lengthBytes, 0, lengthBytes.Length);
            await clientStream.WriteAsync(JsonAsBytes, 0, JsonAsBytes.Length);
        }

        public static ResponseInfo UnknownRequest()
        {
            return new ResponseInfo { IsSuccess = false, Message = "ERROR: Unknow request!"};
        }
    }
}
