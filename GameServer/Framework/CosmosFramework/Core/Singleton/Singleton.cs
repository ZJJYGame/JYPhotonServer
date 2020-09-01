﻿using System;
namespace Cosmos
{
    /// <summary>
    /// 通用继承形单例；
    /// 可选实现IConstruction接口;
    /// 若实现IConstruction，则对象被new完成后，自动调用OnConstruction方法；
    /// </summary>
    /// <typeparam name="TDerived">继承自此单例的可构造类型</typeparam>
    public abstract class Singleton<TDerived> : IDisposable
        where TDerived : class, new()
    {
        protected static TDerived instance;
        public static TDerived Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TDerived();
                    if (instance is IConstruction)
                        (instance as IConstruction).OnConstruction();
                }
                return instance;
            }
        }
        /// <summary>
        /// 非空虚方法，IDispose接口
        /// </summary>
        public virtual void Dispose()
        {
            instance = default(TDerived);
        }
    }

}
