using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 网络事件模块；
    /// </summary>
    /// <typeparam name="TKey">key的类型</typeparam>
    /// <typeparam name="TValue">value的类型</typeparam>
    /// <typeparam name="TDerived">派生类的类型</typeparam>
    public class EventBase<TKey, TValue,TDerived>:ConcurrentSingleton<TDerived>
        where TDerived:EventBase<TKey,TValue,TDerived>,new()
        where TValue : class
    {
        ConcurrentDictionary<TKey, List<Action<TValue>>> eventDict = new ConcurrentDictionary<TKey, List<Action<TValue>>>();
        public virtual void AddEventListener(TKey key, Action<TValue> handler)
        {
            if (eventDict.ContainsKey(key))
                eventDict[key].Add(handler);
            else
            {
                List<Action<TValue>> handlerSet = new List<Action<TValue>>();
                handlerSet.Add(handler);
                eventDict.TryAdd(key, handlerSet);
            }
        }
        public virtual void RemoveEventListener(TKey key, Action<TValue> handler)
        {
            if (eventDict.ContainsKey(key))
            {
                var handlerSet = eventDict[key];
                handlerSet.Remove(handler);
                if(handlerSet.Count <= 0)
                {
                    List<Action<TValue>> removeHandlerSet;
                    eventDict.TryRemove(key, out removeHandlerSet);
                }
            }
        }
        public  bool HasEventListened(TKey key)
        {
            return eventDict.ContainsKey(key);
        }
        public void Dispatch(TKey key, TValue value)
        {
            if (eventDict.ContainsKey(key))
            {
                var handlerSet = eventDict[key];
                int length = handlerSet.Count;
                for (int i = 0; i < length; i++)
                {
                    handlerSet[i]?.Invoke(value);
                }
            }
        }
        public void Dispatch(TKey key)
        {
            Dispatch(key, null);
        }
    }
}
