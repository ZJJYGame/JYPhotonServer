using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace AscensionServer
{
    /// <summary>
    /// 这是一个缓存容器，利用hash存储key,dictionary存储 T value。
    /// </summary>
    public class Cache<TKey,TValue>:IKeyValue<TKey,TValue>
    {
        #region Properties
        Action<TValue> OnAdd;
        Action<TValue> OnRemove;
        Action<TValue> OnUpdate;
        HashSet<TKey> keyHash;
        public HashSet<TKey> KeyHash { get { return keyHash; } set { keyHash = value; } }
        Dictionary<TKey, TValue> dict;
        public Dictionary<TKey,TValue> Dict { get { return dict; } set { dict = value; } }
        public int Count { get { return keyHash.Count; } }
        #endregion
        #region Methods
        public Cache(Action<TValue> addHandler, Action<TValue> removeHandler, Action<TValue> updateHandler) : this()
        {
            this.OnAdd = addHandler;
            this.OnRemove = removeHandler;
            this.OnUpdate = updateHandler;
        }
        public Cache()
        {
            keyHash = new HashSet<TKey>();
            dict = new Dictionary<TKey, TValue>();
        }
        public void Clear()
        {
            keyHash.Clear();
            dict.Clear();
        }

        public TValue this[TKey key]
        {
            get
            {
                bool exist = keyHash.Contains(key);
                TValue value = default(TValue);
                if (exist)
                {
                    value = dict[key];
                }
                return value;
            }
            set
            {
                bool exist = keyHash.Contains(key);
                if (exist)
                {
                    dict[key] = value;
                }
            }
        }
        public List<TValue> GetValuesList()
        {
            var values = new List<TValue>(dict.Values);
            return values;
        }
        public HashSet<TValue> GetValuesHashSet()
        {
            var values = new HashSet<TValue>(dict.Values);
            return values;
        }
        public bool TryGetValue(TKey key, out TValue value)
        {
            bool exist = dict.TryGetValue(key, out value);
            return exist;
        }
        public bool ContainsKey(TKey key)
        {
            bool exist = keyHash.Contains(key);
            return exist;
        }
        public bool TryRemove(TKey key)
        {
            bool exist = keyHash.Contains(key);
            if (exist)
            {
                var arg = dict[key];
                dict.Remove(key);
                keyHash.Remove(key);
                OnRemove?.Invoke(arg);
                return true;
            }
            else
                return false;
        }
        public bool TryAdd(TKey key, TValue Value)
        {
            bool exist = keyHash.Contains(key);
            if (!exist)
            {
                keyHash.Add(key);
                dict.Add(key, Value);
                OnAdd?.Invoke(Value);
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 将指定键的现有值与指定值进行比较，如果相等，则用第三个值更新该键。
        /// </summary>
        /// <param name="key"> 其值将与 comparisonValue 进行比较并且可能被替换的键。</param>
        /// <param name="newValue">当比较结果相等时，该值将替换具有指定 key 的元素的值。</param>
        /// <param name="comparsionValue"> 与具有指定 key 的元素的值进行比较的值。</param>
        /// <returns>如果具有 true 的值与 key 相等且被替换为 comparisonValue，则为 newValue；否则为 false。</returns>
        public bool TryUpdate(TKey key, TValue newValue, TValue comparsionValue)
        {
            bool exist = keyHash.Contains(key);
            if (exist)
            {
                var oldVar = dict[key];
                if (oldVar.Equals(comparsionValue))
                {
                    dict[key] = newValue;
                    OnUpdate?.Invoke(newValue);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        public bool TryUpdate(TKey key, TValue value)
        {
            bool exist = keyHash.Contains(key);
            if (exist)
            {
                dict[key] = value;
                OnUpdate?.Invoke(value);
                return true;
            }
            else
                return false;
        }
        #endregion
    }
}
