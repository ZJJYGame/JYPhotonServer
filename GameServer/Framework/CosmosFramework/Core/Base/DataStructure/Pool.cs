using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class Pool<T>:IEnumerable
    {
        public int Count { get { return objects.Count; } }
        readonly Queue<T> objects = new Queue<T>();
        readonly Func<T> objectGenerator;
        readonly Action<T> objectDispose;
        public Pool(Func<T> objectGenerator, Action<T> objectDispose = null)
        {
            this.objectGenerator = objectGenerator;
            this.objectDispose = objectDispose;
        }
        public T Spawn()
        {
            if (objects.Count > 0)
                return objects.Dequeue() ;
            else
                return objectGenerator();
        }
        public void Despawn(T obj)
        {
            objectDispose?.Invoke(obj);
            objects.Enqueue(obj);
        }
        public IEnumerator GetEnumerator()
        {
            return objects.GetEnumerator();
        }
        public void Clear()
        {
            objects.Clear();
        }
    }
}
