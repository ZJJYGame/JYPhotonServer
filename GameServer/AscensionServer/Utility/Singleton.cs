using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 非mono单例的基类new()约束
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : IDisposable
        where T : new()
    {
        static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = new T();
                return instance;
            }
        }
        /// <summary>
        /// 空虚方法，IDispose接口
        /// </summary>
        public virtual void Dispose() { }

        protected virtual void ClearSingleton()
        {
            instance = default(T);
        }
    }
    /// <summary>
    /// 多线程单例基类，内部包含线程锁
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MultiThreadSingleton<T> : IDisposable
       where T : new()
    {
        static T instance;
        static readonly object locker = new object();
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                            instance = new T();
                    }
                }
                return instance;
            }
        }
        /// <summary>
        /// 空虚方法，IDispose接口
        /// </summary>
        public virtual void Dispose() { }

    }
}
