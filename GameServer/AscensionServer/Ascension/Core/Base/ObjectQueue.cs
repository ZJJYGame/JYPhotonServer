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
        where T : class, IDisposable, new()
    {
        ConcurrentQueue<T> objQueue;
        readonly bool enqueueDispose;
        public int Count { get { return objQueue.Count; } }
        /// <summary>
        /// 构造函数；
        /// </summary>
        /// <param name="enqueueDispose">入队时是否释放</param>
        public ObjectQueue(bool enqueueDispose = true)
        {
            this.enqueueDispose = enqueueDispose;
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
            if (enqueueDispose)
                obj.Dispose();
            objQueue.Enqueue(obj);
        }
        public IEnumerator GetEnumerator()
        {
            return objQueue.GetEnumerator();
        }
    }
}
