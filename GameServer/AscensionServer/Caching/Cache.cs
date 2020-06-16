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
    /// <typeparam name="T"></typeparam>
    public class Cache<T>
    {
        #region Properties
        Action<T> OnAdd;
        Action<T> OnRemove;
        Action<T> OnUpdate;
        HashSet<string> keyHash;
        public HashSet<string> KeyHash { get { return keyHash; } set { keyHash = value; } }
        Dictionary<string, T> dict;
        public Dictionary<string, T> Dict { get { return dict; } set { dict = value; } }
        public int Count { get { return keyHash.Count; } }
        #endregion
        #region Methods
        public Cache(Action<T> addHandler, Action<T> removeHandler, Action<T> updateHandler) : this()
        {
            this.OnAdd = addHandler;
            this.OnRemove = removeHandler;
            this.OnUpdate = updateHandler;
        }
        public Cache()
        {
            keyHash = new HashSet<string>();
            dict = new Dictionary<string, T>();
        }
        public void Clear()
        {
            keyHash.Clear();
            dict.Clear();
        }
        public bool IsExists(string key)
        {
            bool exist = keyHash.Contains(key);
            return exist;
        }
        public bool Add(string key, T arg)
        {
            bool exist = keyHash.Contains(key);
            if (!exist)
            {
                keyHash.Add(key);
                dict.Add(key, arg);
                OnAdd?.Invoke(arg);
                return true;
            }
            else
                return false;
        }
        public bool Remove(string key)
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
        public bool Set(string key, T arg)
        {
            bool exist = keyHash.Contains(key);
            if (exist)
            {
                dict[key] = arg;
                OnUpdate?.Invoke(arg);
                return true;

            }
            else
                return false;
        }
        public bool TryGetValue(string key, out T value)
        {
            bool exist = dict.TryGetValue(key, out value);
            return exist;
        }
        public T this[string key]
        {
            get
            {
                bool exist = keyHash.Contains(key);
                T value = default(T);
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
        public List<T> GetValuesList()
        {
            var values = new List<T>(dict.Values);
            return values;
        }
        public HashSet<T> GetValuesHashSet()
        {
            var values = new HashSet<T>(dict.Values);
            return values;
        }
        #endregion
    }
}
