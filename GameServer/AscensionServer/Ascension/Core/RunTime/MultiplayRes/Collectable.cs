using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    public class Collectable:IDisposable
    {
        static ConcurrentPool<Collectable> collectablePool;
        static Collectable()
        {
            collectablePool = new ConcurrentPool<Collectable>(() => new Collectable(),(c)=> { c.Dispose(); }); 
        }

        public Collectable()
        {
            elementDict = new Dictionary<int, CollectableElement>();
        }
        /// <summary>
        /// 表示全局Id
        /// </summary>
        public int Id { get; private set; }
        public int Count { get { return elementDict.Count; } }
        class CollectableElement
        {
            /// <summary>
            /// 内部生成的Id
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// 位置信息
            /// </summary>
            public FixTransform FixTransform { get; set; }
        }
        Dictionary<int, CollectableElement> elementDict;
        public void Dispose()
        {
            elementDict.Clear();
            Id = 0;
        }
        public static Collectable Create(int id)
        {
            var c = collectablePool.Spawn();
            c.Id = id;
            return c;
        }
        public static void Release(Collectable collectable)
        {
            collectablePool.Despawn(collectable);
        }
    }
}
