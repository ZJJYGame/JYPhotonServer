/*
*Author:  Don
*Since :	2020-05-05
*Description : 单例基类，包含线程安全类型
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos
{
    /// <summary>
    /// 普通单例；线程不安全;
    /// 若派生类实现了IBehaviour，则初始化时候自动调用IBehaviour的方法；
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : IDisposable
        where T : class,new()
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                    if (instance is IBehaviour)
                        (instance as IBehaviour).OnInitialization();
                }
                return instance;
            }
        }
        /// <summary>
        /// 非空虚方法，IDispose接口
        /// </summary>
        public virtual void Dispose() {
            if (instance is IBehaviour)
                (instance as IBehaviour).OnTermination();
            instance = default(T); }
    }
    /// <summary>
    /// 线程安全并发单例；
    /// 若派生类实现了IBehaviour，则初始化时候自动调用IBehaviour的方法；
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
            instance = default(T); }
    }
}
