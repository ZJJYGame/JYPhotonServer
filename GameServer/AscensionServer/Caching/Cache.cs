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
    /// 使用了读写分离锁确保线程安全。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Cache<T>
    {
        #region Properties
        Action<T> OnAdd;
        Action<T> OnRemove;
        Action<T> OnUpdate;
        ReaderWriterLockSlim locker;
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
        }
        public Cache()
        {
            locker = new ReaderWriterLockSlim();
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
            locker.EnterReadLock();
            bool exist = keyHash.Contains(key);
            locker.ExitReadLock();
            return exist;
        }
        public bool Add(string key, T arg)
        {
            locker.EnterWriteLock();
            if (!keyHash.Contains(key))
            {
                keyHash.Add(key);
                dict.Add(key, arg);
                OnAdd?.Invoke(arg);
                return true;
            }
            else
            {
                AscensionServer._Log.Info("=========== can't add key : " + key + " arg :" + arg.ToString() + " into the cache ! =========== ");
            }
            locker.ExitWriteLock();
            return false;
        }
        public bool Remove(string key)
        {
            locker.EnterWriteLock();
            if (keyHash.Contains(key))
            {
                var arg = dict[key];
                dict.Remove(key);
                keyHash.Remove(key);
                OnRemove?.Invoke(arg);
                return true;
            }
            else
            {
                AscensionServer._Log.Info("=========== can't remove key : " + key + " from the cache ! =========== ");
            }
            locker.ExitWriteLock();
            return false;
        }
        public bool Set(string key, T arg)
        {
            locker.EnterWriteLock();
            if (keyHash.Contains(key))
            {
                dict[key] = arg;
                OnUpdate?.Invoke(arg);
                return true;
            }
            else
            {
                AscensionServer._Log.Info("=========== can't  update arg , key: " + key + "  is not exists in the cache ! =========== ");
            }
            locker.ExitWriteLock();
            return false;
        }
        public T Get(string key)
        {
            locker.EnterReadLock();
            T value = default(T);
            if (keyHash.Contains(key))
            {
                value = dict[key];
            }
            locker.ExitReadLock();
            return value;
        }
        public T this[string key]
        {
            get
            {
                bool exist = keyHash.Contains(key);
                T value = default(T);
                if (exist)
                {
                    locker.EnterReadLock();
                    try
                    {
                        value = dict[key];
                    }
                    finally
                    {
                        locker.ExitReadLock();
                    }
                }
                return value;
            }
            set
            {
                bool exist = keyHash.Contains(key);
                if (exist)
                {
                    locker.EnterWriteLock();
                    try
                    {
                        dict[key] = value;
                    }
                    finally
                    {
                        locker.ExitWriteLock();
                    }
                }
            }
        }
        public List<T> GetValuesList()
        {
            locker.EnterReadLock();
            var values = new List<T>(dict.Values);
            locker.ExitReadLock();
            return values;
        }
        public HashSet<T> GetValuesHashSet()
        {
            locker.EnterReadLock();
            var values = new HashSet<T>(dict.Values);
            locker.ExitReadLock();
            return values;
        }
        #endregion
    }
}
