using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.CommunicationModels
{
    public class RequestInfo
    {
        public MessageType MessageType { get; set; }
        public string From { get; set; }
        public byte[] Data { get; set; }
    }
}
