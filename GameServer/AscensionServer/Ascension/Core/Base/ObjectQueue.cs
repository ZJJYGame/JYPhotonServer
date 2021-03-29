using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using System.Collections;

namespace AscensionServer
{
    public class ObjectQueue<T> : IEnumerable, IReference
        where T : class, new()
    {
        ConcurrentQueue<T> objQueue;
        public int Count { get { return objQueue.Count; } }
        public ObjectQueue()
        {
            objQueue = new ConcurrentQueue<T>();
        }
        public void Clear()
        {
            objQueue.Clear();
        }
        public T Dequeue()
        {
            T obj = default;
            if (objQueue.Count <= 0)
            {
                obj = new T();
            }
            else
                objQueue.TryDequeue(out obj);
            return obj;
        }
        public void Enqueue(T obj)
        {
            objQueue.Enqueue(obj);
        }
        public IEnumerator GetEnumerator()
        {
            return objQueue.GetEnumerator();
        }
    }
}
