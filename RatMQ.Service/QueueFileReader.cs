using System.Collections.Generic;
using System.IO;
using RatMQ.Service.Utils;

namespace RatMQ.Service
{
    static class QueueFileReader
    {
        public static List<QueueDescription> GetQueueDescriptions()
        {
            if (!File.Exists("queues.config")) return null;
            var queuesFileText = File.ReadAllText("queues.config");
            var queueFileDescription = JsonSerializer.FromJson<QueueFileDescription>(queuesFileText);

            return queueFileDescription.Queues;
        }
    }

    class QueueFileDescription
    {
        public List<QueueDescription> Queues { get; set; }
    }

    class QueueDescription
    {
        public string Name { get; set; }
    }
}
