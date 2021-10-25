namespace RatMQ.Service.Domain
{
    public class BrokerMessage
    {
        public string Body { get; set; }

        public bool Sended { get; set; }

        public BrokerMessage()
        {
            Sended = false;
        }
    }
}
