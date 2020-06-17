/*
*Author : Don
*Since 	:2020-04-18
*Description  : NHibernate Manager抽象基类
*/
using System;
using NHibernate;
using NHibernate.Criterion;
using Cosmos;
using System.Collections;
using System.Collections.Generic;
namespace AscensionServer
{
    /// <summary>
    /// 泛型单例基类
    /// </summary>
    /// <typeparam name="E">泛型约束为当前类的子类</typeparam>
    public class NHManager : INHManager, IBehaviour
    {
        /// <summary>
        ///空的虚方法，在当前单例对象为空初始化时执行一次
        /// </summary>
        public virtual void OnInitialization() { }
        /// <summary>
        /// 空的虚方法，在当前单例对象被销毁时执行一次
        /// </summary>
        public virtual void OnTermination() { }
        /// <summary>
        ///         /// <summary>
        /// 可覆写非空虚函数;
        /// 添加数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">具体数据</param>
        /// <returns>返回一个完整带有主键ID的对象</returns>
        public virtual T Insert<T>(T data) where T : class, new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(data);
                    transaction.Commit();
                    return data;
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
        public virtual T Get<T>(object key)
            where T : new()
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
        /// 条件获得数据
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <typeparam name="columnName">表字段的名称</typeparam>
        /// <typeparam name="K">查找类型</typeparam>
        /// <param name="key">查找索引字段</param>
        /// <param name="key"></param>
        public virtual T CriteriaSelect<T>(params NHCriteria[] columns)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                for (int i = 0; i < columns.Length; i++)
                {
                    criteria.Add(Restrictions.Eq(columns[i].PropertyName, columns[i].Value));
                }
                T data = criteria.UniqueResult<T>();
                return data;
            }
        }
        /// <summary>
        /// 条件查找符合的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public virtual IList<T>  CriteriaSelectList<T>(params NHCriteria[] columns)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                for (int i = 0; i < columns.Length; i++)
                {
                    criteria.Add(Restrictions.Eq(columns[i].PropertyName, columns[i].Value));
                }
                return criteria.List<T>();
            }
        }
        /// <summary>
        /// 多条件验证，SQL语句为Equal
        /// </summary>
        /// <typeparam name="T">需要验证的类型</typeparam>
        /// <param name="columns">column类对象</param>
        /// <returns>验证是否成功</returns>
        public virtual bool Verify<T>(params NHCriteria[] columns)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                for (int i = 0; i < columns.Length; i++)
                {
                    criteria.Add(Restrictions.Eq(columns[i].PropertyName, columns[i].Value));
                }
                T data = criteria.UniqueResult<T>();
                return data!=null;
            }
        }
        /// <summary>
        /// 查找并返回所有条件对象的总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public virtual int Count<T>(params NHCriteria[] columns)
            where T : class
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria<T>();
                for (int i = 0; i < columns.Length; i++)
                {
                    criteria.Add(Restrictions.Eq(columns[i].PropertyName, columns[i].Value));
                }
               return criteria.List<T>().Count;
            }
        }
        /// <summary>
        /// 可覆写非空虚函数;
        /// 移除数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">具体数据</param>
        public virtual void Delete<T>(T data) where T : new()
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
        public virtual void Update<T>(T data) where T : new()
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
