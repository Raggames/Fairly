using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Systems.Pool
{
    public class GenericPool<T> where T : IPoolable
    {
        public int maxItems { get; private set; }
        public int currentItems { get; private set; }

        protected ConcurrentBag<T> items = new ConcurrentBag<T>();

        public GenericPool(int maxItems = -1)
        {
            this.maxItems = maxItems;
            items = new ConcurrentBag<T>();
        }

        public T Get()
        {
            if(items.TryTake(out T item))
            {
                return item;
            }
            else
            {
                if(maxItems == -1 || currentItems < maxItems)
                {
                    currentItems++;
                    return (T)Activator.CreateInstance(typeof(T));
                }
                else
                {
                    throw new Exception($"Pool is full ({items.Count} items) !");
                }
            }
        }

        public void Release(T item)
        {
            if (maxItems == -1 || items.Count < maxItems)
            {
                item.Clear();
                items.Add(item);
            }
            else
            {
                throw new Exception($"Pool is full ({items.Count} items) !");
            }
        }
    }

    public interface IPoolable 
    {
        void Clear();
    }
}
