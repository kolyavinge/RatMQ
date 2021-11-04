using System;
using System.Collections.Generic;
using System.Text;
using RatMQ.Contracts;

namespace RatMQ.Client
{
    public interface IConsumer<TMessage>
    {
        void ConsumeMessage(TMessage message, ConsumeMessageResult result);
    }

    public class ConsumeMessageResult
    {
        private ConnectionContext _connectionContext;
        private readonly string _messageId;

        internal ConsumeMessageResult(ConnectionContext connectionContext, string messageId)
        {
            _connectionContext = connectionContext;
            _messageId = messageId;
        }

        public void Commit()
        {
            var request = new CommitMessageRequestData
            {
                ClientId = _connectionContext.ClientId,
                MessageId = _messageId
            };
            _connectionContext.SendToBroker<CommitMessageResponseData>(request);
        }
    }
}
