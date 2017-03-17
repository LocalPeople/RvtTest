using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rvt2Excel
{
    class CacheQueue<TKey, TValue> where TKey : IComparable<TKey>
    {
        private int capacity;
        private Queue<TKey> keysQueue;
        private Queue<TValue> valuesQueue;

        public CacheQueue(int capacity)
        {
            this.capacity = capacity;
            keysQueue = new Queue<TKey>(capacity);
            valuesQueue = new Queue<TValue>(capacity);
        }

        public bool Contains(TKey key, out TValue value)
        {
            TKey[] keys = keysQueue.ToArray();
            TValue[] values = valuesQueue.ToArray();

            int i = keys.Length - 1;
            for (; i >= 0; i--)
            {
                if (keys[i].CompareTo(key) == 0)
                {
                    break;
                }
            }

            value = i >= 0 ? values[i] : default(TValue);
            return i >= 0 ? true : false;
        }

        public void Cache(TKey key, TValue value)
        {
            if (keysQueue.Count >= capacity - 1)
            {
                keysQueue.Dequeue();
                valuesQueue.Dequeue();
            }
            keysQueue.Enqueue(key);
            valuesQueue.Enqueue(value);
        }
    }
}
