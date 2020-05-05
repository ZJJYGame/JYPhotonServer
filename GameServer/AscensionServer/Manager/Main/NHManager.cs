/*
*Author : Don
*Since 	:2020-04-18
*Description  : NHibernate Manager抽象基类
*/
using System;
using NHibernate;
namespace AscensionServer
{
    /// <summary>
    /// 泛型抽象单例基类
    /// </summary>
    /// <typeparam name="E">泛型约束为当前类的子类</typeparam>
    public abstract class NHManager <E>:Singleton<E>, IManager
        where E:NHManager<E>,new()
    {
        /// <summary>
        /// 可覆写非空虚函数;
        /// 添加数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">具体数据</param>
        public virtual void Add<T>(T data) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(data);
                    transaction.Commit();
                }
            }
        }
        /// <summary>
        /// 可覆写非空虚函数;
        /// 获取数据
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <typeparam name="K">查找类型</typeparam>
        /// <param name="key">查找索引字段</param>
        /// <returns></returns>
        public virtual T Get<T, K>(K key)
            where T : new()
            where K : IComparable<K>
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                     T data = session.Get<T>(key);
                    transaction.Commit();
                    return data;
                }
            }
        }

        /// <summary>
        /// 可覆写非空虚函数;
        /// 移除数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">具体数据</param>
        public virtual void Remove<T>(T data) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(data);
                    transaction.Commit();
                }
            }
        }
        /// <summary>
        /// 可覆写非空虚函数;
        /// 更新数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">具体数据</param>
        public virtual void Updata<T>(T data) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Update(data);
                    transaction.Commit();
                }
            }

        }
    }
}
