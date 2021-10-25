using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatMQ.Service.Domain
{
    public class Client
    {
        public string ClientId { get; set; }
        public string ClientIp { get; set; }
        public int ClientPort { get; set; }
    }
}
