using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatMQ.Service.Domain
{
    public class Consumer
    {
        public string ClientId { get; set; }

        public bool IsReadyToConsume { get; set; }

        public string QueueName { get; set; }

        public Consumer()
        {
            IsReadyToConsume = true;
        }
    }
}
