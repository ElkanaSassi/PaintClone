using System;
using System.IO;
using SharedModels.CommunicationModels;

namespace Server.Logger
{
    public class ServerLogger
    {
        private static readonly object _lock = new object(); // readonly - cant change obj reference. 
        private const string LogPath = "server_log.log";

        public static void Log(MessageType requestType, string from, bool IsSuccessful)
        {
            lock (_lock) // using the locking mechanism to ensure thread safety. (prevanting 2+ threads to modify the log file at the same time)
            {
                File.AppendAllText(LogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] From: {from}. RequestType: {requestType}. Seccess Status: {IsSuccessful}\n");
            }
        }
    }
}
