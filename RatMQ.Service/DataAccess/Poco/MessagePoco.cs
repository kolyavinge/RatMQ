using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatMQ.Service.DataAccess.Poco
{
    public class MessagePoco
    {
        public string Id { get; set; }

        public string QueueName { get; set; }

        public KeyValuePair<string, object>[] Headers { get; set; }

        public byte[] Body { get; set; }
    }
}
