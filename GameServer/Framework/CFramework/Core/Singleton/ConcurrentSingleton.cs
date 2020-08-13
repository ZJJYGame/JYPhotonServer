using System;
namespace Cosmos
{
    /// <summary>
    /// 多线程单例基类，内部包含线程锁;
    /// 可选实现IBehaviour接口。若实现IBehaviour，则可此接口的内部方法会被初始化与销毁时自动被调用；
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ConcurrentSingleton<T> : IDisposable
              where T : class, new()
    {
        protected static T instance;
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
                        {
                            instance = new T();
                            if (instance is IBehaviour)
                                (instance as IBehaviour).OnInitialization();
                        }
                    }
                }
                return instance;
            }
        }
        /// <summary>
        ///非空虚方法，IDispose接口
        /// </summary>
        public virtual void Dispose()
        {
            if (instance is IBehaviour)
                (instance as IBehaviour).OnTermination();
            instance = default(T);
        }
    }
}
