using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Client
{
    public interface IConsumer<TMessage>
    {
        ConsumeMessageResult ConsumeMessage(TMessage message);
    }

    public class ConsumeMessageResult
    {
        public bool Success { get; set; }
    }
}
