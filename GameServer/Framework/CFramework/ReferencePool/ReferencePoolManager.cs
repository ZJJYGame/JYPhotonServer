using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos
{
    public sealed class ReferencePoolManager :IBehaviour
    {
        public void OnInitialization() { }
        public void OnTermination() { }
        /// <summary>
        /// 单个引用池上线
        /// </summary>
        public static readonly short _ReferencePoolCapcity= 20000;
        Dictionary<Type, ReferenceSpawnPool> referencePool = new Dictionary<Type, ReferenceSpawnPool>();
        public int GetPoolCount<T>() 
            where T : class, IReference, new()
        {
            try
            {
                    return referencePool[typeof(T)].ReferenceCount;
            }
            catch (Exception)
            {
                throw new Exception("Type :" + typeof(T).FullName + " not register in reference pool");
            }
        }
        public T Spawn<T>() 
            where T:class ,IReference ,new()
        {
            Type type = typeof(T);
            if (!referencePool.ContainsKey(type))
            {
                referencePool.Add(type, new ReferenceSpawnPool());
            }
            return referencePool[type].Spawn<T>() as T;
        }
        public IReference Spawn(Type type)
        {
            if (!referencePool.ContainsKey(type))
            {
                referencePool.Add(type, new ReferenceSpawnPool());
            }
            return referencePool[type].Spawn(type);
        }
        public void Despawn(IReference refer)
        {
            Type type = refer.GetType();
            if (!referencePool.ContainsKey(type))
                referencePool.Add(type, new ReferenceSpawnPool());
            referencePool[type].Despawn(refer);
        }
        public void Despawns(params IReference[] refers)
        {
            for (int i = 0; i < refers.Length; i++)
            {
                Type type = refers[i].GetType();
                if (!referencePool.ContainsKey(type))
                    referencePool.Add(type, new ReferenceSpawnPool());
                referencePool[type].Despawn(refers[i]);
            }
        }
        public void Despawns<T>(List<T> refers)
            where T:class ,IReference,new()
        {
            Type type = typeof(T);
            if (!referencePool.ContainsKey(type))
            {
                referencePool.Add(type, new ReferenceSpawnPool());
            }
            if (refers.Count > 0)
            {
                for (int i = 0; i < refers.Count; i++)
                {
                    referencePool[type].Despawn(refers[i]);
                }
                refers.Clear();
            }
        }
        public void Despawns<T>(T[] refers)
            where T :class,IReference,new()
        {
            Type type = typeof(T);
            if (!referencePool.ContainsKey(type))
            {
                referencePool.Add(type, new ReferenceSpawnPool());
            }
            for (int i = 0; i < refers.Length; i++)
            {
                referencePool[type].Despawn(refers[i]);
            }
        }
        public void Clear(Type type)
        {
            if (referencePool.ContainsKey(type))
            {
                referencePool[type].Clear();
            }
        }
        public void Clear<T>()
            where T : class, IReference, new()
        {
            Type type = typeof(T);
            Clear(type);
        }
        public void ClearAll()
        {
            foreach (var referPool in referencePool)
            {
                referPool.Value.Clear();
            }
        }
    }
}