using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SharedModels.CommunicationModels;

namespace Client.Network
{
    public static class StreamHelper
    {
        public static async Task SendMessageAsync(Stream stream, RequestInfo request)
        {
            string JsonPayload = JsonSerializer.Serialize(request);
            byte[] JsonPayloadAsBytes = Encoding.UTF8.GetBytes(JsonPayload);

            await stream.WriteAsync(BitConverter.GetBytes(JsonPayloadAsBytes.Length), 0, 4);
            await stream.WriteAsync(JsonPayloadAsBytes, 0, JsonPayloadAsBytes.Length);
        }

        public static async Task<string> ReceiveMessageAsync(Stream stream)
        {
            byte[] lengthBuffer = new byte[4];
            await stream.ReadExactlyAsync(lengthBuffer, 0, 4);
            int length = BitConverter.ToInt32(lengthBuffer, 0);

            byte[] buffer = new byte[length];
            await stream.ReadExactlyAsync(buffer, 0, length);
            string bufferAsString = Encoding.UTF8.GetString(buffer);

            return bufferAsString;
        }
    }
}
