using System;
using System.Threading;
using RatMQ.Client;

namespace StarterApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(1000); // broker up delay
            var brokerIp = "127.0.0.1";
            var brokerPort = 55555;
            var clientPort = 55556;
            var broker = BrokerConnector.Connect(brokerIp, brokerPort, clientPort);
            var queue1 = broker.IsQueueExist("queue1") ? broker.GetQueue<Queue1Message>("queue1") : broker.CreateQueue<Queue1Message>("queue1");
            var queue2 = broker.IsQueueExist("queue2") ? broker.GetQueue<Queue2Message>("queue2") : broker.CreateQueue<Queue2Message>("queue2");
            queue1.AddConsumer(new Queue1Consumer());
            queue2.AddConsumer(new Queue2Consumer());
            for (int i = 1; i <= 50; i++)
            {
                queue1.SendMessage(new Queue1Message
                {
                    IntField = i,
                    StringField = "queue1_" + i,
                    InnerField = new Inner { Value = 555 }
                });
                Console.WriteLine($"Message queue1 {i} sended");
                queue2.SendMessage(new Queue2Message
                {
                    IntField = i,
                    StringField = "queue2_" + i,
                    InnerField = new Inner { Value = 555 }
                });
                Console.WriteLine($"Message queue2 {i} sended");
            }

            Console.ReadKey();
        }
    }

    class Queue1Consumer : IConsumer<Queue1Message>
    {
        public void ConsumeMessage(Queue1Message message, ConsumeMessageResult result)
        {
            result.Commit();
            Console.WriteLine($"Message queue1 consumed: {message}");
        }
    }

    class Queue2Consumer : IConsumer<Queue2Message>
    {
        public void ConsumeMessage(Queue2Message message, ConsumeMessageResult result)
        {
            result.Commit();
            Console.WriteLine($"Message queue2 consumed: {message}");
        }
    }

    class Queue1Message
    {
        public int IntField { get; set; }
        public string StringField { get; set; }
        public Inner InnerField { get; set; }

        public override string ToString()
        {
            return $"IntField: {IntField}, StringField: {StringField}, InnerField: {InnerField.Value}";
        }
    }

    class Queue2Message
    {
        public int IntField { get; set; }
        public string StringField { get; set; }
        public Inner InnerField { get; set; }

        public override string ToString()
        {
            return $"IntField: {IntField}, StringField: {StringField}, InnerField: {InnerField.Value}";
        }
    }

    class Inner
    {
        public int Value { get; set; }
    }
}
