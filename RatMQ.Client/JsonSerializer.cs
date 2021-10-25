using System;

namespace RatMQ.Client
{
    static class JsonSerializer
    {
        public static string ToJson(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public static T FromJson<T>(string str)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
        }

        public static object FromJson(Type type, string str)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(str, type);
        }
    }
}
