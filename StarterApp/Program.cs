using System;
using System.Collections.Generic;
using System.Threading;
using RatMQ.Client;
using RatMQ.Contracts;

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
            var queue1 = broker.IsQueueExist("queue1") ? broker.GetQueue("queue1") : broker.CreateQueue("queue1");
            var queue2 = broker.IsQueueExist("queue2") ? broker.GetQueue("queue2") : broker.CreateQueue("queue2");
            queue1.AddConsumer(new Queue1Consumer());
            queue2.AddConsumer(new Queue2Consumer());
            for (int i = 1; i <= 50; i++)
            {
                queue1.SendMessage(
                    new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("queue1", i) },
                    BinarySerializer.ToBinary(new Queue1Message
                    {
                        IntField = i,
                        StringField = "queue1_" + i,
                        InnerField = new Inner { Value = 555 }
                    }));
                Console.WriteLine($"Message queue1 {i} sended");

                queue2.SendMessage(
                    new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("queue2", i) },
                    BinarySerializer.ToBinary(new Queue2Message
                    {
                        IntField = i,
                        StringField = "queue2_" + i,
                        InnerField = new Inner { Value = 555 }
                    }));
                Console.WriteLine($"Message queue2 {i} sended");
            }

            Console.ReadKey();
        }
    }

    class Queue1Consumer : IConsumer
    {
        public void ConsumeMessage(ClientMessage clientMessage, ConsumeMessageResult result)
        {
            result.Commit();
            var message = (Queue1Message)BinarySerializer.FromBinary(clientMessage.Body);
            Console.WriteLine($"Message queue1 consumed: {message}. Header: {clientMessage.Headers[0]}");
        }
    }

    class Queue2Consumer : IConsumer
    {
        public void ConsumeMessage(ClientMessage clientMessage, ConsumeMessageResult result)
        {
            result.Commit();
            var message = (Queue2Message)BinarySerializer.FromBinary(clientMessage.Body);
            Console.WriteLine($"Message queue2 consumed: {message}. Header: {clientMessage.Headers[0]}");
        }
    }

    [Serializable]
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

    [Serializable]
    class Queue2Message
    {
        public int IntField { get; set; }
        public string StringField { get; set; }
        public Inner InnerField { get; set; }
        public byte[] BigArray { get; set; }

        public Queue2Message()
        {
            BigArray = new byte[1 * 1024 * 1024];
        }

        public override string ToString()
        {
            return $"IntField: {IntField}, StringField: {StringField}, InnerField: {InnerField.Value}";
        }
    }

    [Serializable]
    class Inner
    {
        public int Value { get; set; }
    }
}
