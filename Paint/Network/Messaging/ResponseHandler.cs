using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SharedModels.CommunicationModels;


namespace Client.Network.Messaging
{
    public static class ResponseHandler
    {
        public static async Task<ResponseInfo> ReadResponseAsync(Stream stream)
        {
            string buffer = await StreamHelper.ReceiveMessageAsync(stream);
            return JsonSerializer.Deserialize<ResponseInfo>(buffer);
        }
    }
}
