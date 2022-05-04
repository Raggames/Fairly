using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Systems.Pool
{
    public static class Factory<T> where T : IPoolable
    {
        public static Dictionary<Type, GenericPool<T>> pools;

        public static void InitFactory()
        {
            pools = new Dictionary<Type, GenericPool<T>>();
        }

        public static T GetItem()
        {
            if (pools == null)
                InitFactory();

            if (pools.ContainsKey(typeof(T)))
            {
                return (T)pools[typeof(T)].Get();
            }
            else
            {
                var pool = new GenericPool<T>(100);
                pools.Add(typeof(T), pool);
                return (T)pools[typeof(T)].Get();
            }
        }

        public static void Release(T item)
        {
            if (pools == null)
                InitFactory();

            if (pools.ContainsKey(typeof(T)))
            {
                pools[typeof(T)].Release(item);
            }
            else
            {
                Debug.LogError("Warning, pool doesn't exist. You are trying to releasing an item that has been created out of the Factory context !");

                var pool = new GenericPool<T>(100);
                pools.Add(typeof(T), pool);
                pool.Release(item);
            }
        }
    }
}
