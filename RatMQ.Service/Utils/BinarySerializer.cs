using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RatMQ.Service.Utils
{
    public static class BinarySerializer
    {
        public static byte[] ToBinary(object obj)
        {
            using var memory = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(memory, obj);

            return memory.ToArray();
        }

        public static object FromBinary(byte[] bytes)
        {
            using var memory = new MemoryStream(bytes);
            var formatter = new BinaryFormatter();
            var obj = formatter.Deserialize(memory);

            return obj;
        }
    }
}
