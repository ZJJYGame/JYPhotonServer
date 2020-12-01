using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
/// <summary>
/// 此静态类为扩展方法工具类；
/// </summary>
public static class NetCoreExtend
{
    /// <summary>
    /// 字典扩展方法，来自移植.NetCore 2.2
    /// </summary>
    public static bool TryAdd<TKey,TValue>(this Dictionary<TKey,TValue>dict,TKey key,TValue value )
    {
        if (dict.ContainsKey(key))
            return false;
        else
        {
             dict.Add(key, value);
            return true;
        }
    }
    public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        if (dict.ContainsKey(key))
        {
            dict[key] = value;
        }
        else
        {
            dict.Add(key, value);
        }
    }
    /// <summary>
    /// 字典扩展方法，来自移植.NetCore 2.2
    /// </summary>
    public static bool Remove<TKey, TValue>(this Dictionary<TKey, TValue> dict,TKey key, out TValue value)
    {
        if (dict.ContainsKey(key))
        {
            value = dict[key];
            dict.Remove(key);
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }
    public static void Clear<TValue>(this ConcurrentQueue<TValue> queue)
    {
        while (queue.Count>0)
        {
            queue.TryDequeue(out _);
        }
    }
    public static  bool Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, out TValue value)
    {
        return dict.TryRemove(key, out value);
    }
}
