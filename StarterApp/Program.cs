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
            var queue = broker.IsQueueExist("testQueue") ?
                broker.GetQueue<TestMessage>("testQueue") : broker.CreateQueue<TestMessage>("testQueue");
            queue.AddConsumer(new TestConsumer());
            for (int i = 1; i <= 100; i++)
            {
                queue.SendMessage(new TestMessage
                {
                    IntField = i,
                    StringField = "StringField " + i,
                    InnerField = new Inner { Value = 555 }
                });
                Console.WriteLine($"Client message {i} has been sended");
            }

            Console.ReadKey();
        }
    }

    class TestConsumer : IConsumer<TestMessage>
    {
        public void ConsumeMessage(TestMessage message, ConsumeMessageResult result)
        {
            Console.WriteLine($"Broker message has been consumed: {message}");
            result.Commit();
        }
    }

    class TestMessage
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
