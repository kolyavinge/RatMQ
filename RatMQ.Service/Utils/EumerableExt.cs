using System;
using System.Collections.Generic;

namespace RatMQ.Service.Utils
{
    public static class EumerableExt
    {
        public static void Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }
}
