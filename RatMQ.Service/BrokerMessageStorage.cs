using System.Collections.Generic;
using System.Linq;
using RatMQ.Service.DataAccess;
using RatMQ.Service.DataAccess.Poco;
using RatMQ.Service.Domain;

namespace RatMQ.Service
{
    public interface IBrokerMessageStorage
    {
        IEnumerable<BrokerMessage> GetUncommitedMessages();

        void SaveUncommitedMessage(BrokerMessage message);

        void CommitMessage(string messageId);
    }

    public class BrokerMessageStorage : IBrokerMessageStorage
    {
        private readonly IDataContext _dataContext;

        public BrokerMessageStorage(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IEnumerable<BrokerMessage> GetUncommitedMessages()
        {
            var commitedMessagesId = _dataContext.GetRepository<CommitedMessagePoco>().GetAll().Select(x => x.MessageId);
            var uncommitedMessages = _dataContext.GetRepository<MessagePoco>().Where(x => !commitedMessagesId.Contains(x.Id));
            var brokerMessages = BrokerMessageConverter.FromPoco(uncommitedMessages);

            return brokerMessages;
        }

        public void SaveUncommitedMessage(BrokerMessage message)
        {
            _dataContext.GetRepository<MessagePoco>().Save(new MessagePoco
            {
                Id = message.Id,
                QueueName = message.QueueName,
                Headers = message.Headers,
                Body = message.Body
            });
        }

        public void CommitMessage(string messageId)
        {
            _dataContext.GetRepository<CommitedMessagePoco>().Save(new CommitedMessagePoco { MessageId = messageId });
        }
    }

    static class BrokerMessageConverter
    {
        public static IEnumerable<BrokerMessage> FromPoco(IEnumerable<MessagePoco> poco)
        {
            return poco.Select(x => new BrokerMessage
            {
                Id = x.Id,
                QueueName = x.QueueName,
                Headers = x.Headers,
                Body = x.Body,
                IsSended = false,
                IsCommited = false
            });
        }
    }
}
