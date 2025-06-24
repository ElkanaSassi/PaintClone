using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.CommunicationModels
{
    public class ResponseInfo
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public byte[] Data { get; set; }
    }
}
