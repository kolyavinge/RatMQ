using System;
using System.Collections.Concurrent;
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

        public static void AddRange<T>(this ConcurrentBag<T> collection, IEnumerable<T> collectionToAdd)
        {
            foreach (var item in collectionToAdd)
            {
                collection.Add(item);
            }
        }
    }
}
