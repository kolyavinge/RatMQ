namespace RatMQ.Service.Domain
{
    public class BrokerMessage
    {
        public string Id { get; set; }

        public string Body { get; set; }

        public bool IsSended { get; set; }

        public bool IsCommited { get; set; }

        public string QueueName { get; set; }

        public BrokerMessage()
        {
            IsSended = false;
            IsCommited = false;
        }
    }
}
