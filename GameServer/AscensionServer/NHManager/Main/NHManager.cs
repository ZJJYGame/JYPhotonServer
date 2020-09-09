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
using System.Threading.Tasks;
namespace AscensionServer
{
    /// <summary>
    /// 泛型单例基类
    /// </summary>
    /// <typeparam name="E">泛型约束为当前类的子类</typeparam>
    public class NHManager
    {
        #region Sync
        /// <summary>
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
        public virtual IList<T> CriteriaSelectList<T>(params NHCriteria[] columns)
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
                return data != null;
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
        /// <summary>
        /// 保存或更新数据
        /// </summary>
        /// <typeparam name="T">无参构造的数据类型</typeparam>
        /// <param name="data">数据对象</param>
        public virtual void SaveOrUpdate<T>(T data) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(data);
                    transaction.Commit();
                }
            }
        }
        /// <summary>
        /// 多条件验证，SQL语句为Like
        /// </summary>
        /// <typeparam name="T">需要验证的类型</typeparam>
        /// <param name="columns">column类对象</param>
        /// <returns>查询到的对象集合</returns>
        public virtual IList<T> CriteriaLike<T>(params NHCriteria[] columns) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                for (int i = 0; i < columns.Length; i++)
                {
                    criteria.Add(Restrictions.Like(columns[i].PropertyName, columns[i].Value));
                }
                return criteria.List<T>();
            }
        }
        /// <summary>
        /// 多条件验证，SQL语句为greater than
        /// </summary>
        /// <typeparam name="T">需要验证的类型</typeparam>
        /// <param name="columns">column类对象</param>
        /// <returns>查询到的对象集合</returns>
        public virtual IList<T> CriteriaGt<T>(params NHCriteria[] columns) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                for (int i = 0; i < columns.Length; i++)
                {
                    criteria.Add(Restrictions.Gt(columns[i].PropertyName, columns[i].Value));
                }
                return criteria.List<T>();
            }
        }
        /// <summary>
        /// 多条件验证，SQL语句为less than
        /// </summary>
        /// <typeparam name="T">需要验证的类型</typeparam>
        /// <param name="columns">column类对象</param>
        /// <returns>查询到的对象集合</returns>
        public virtual IList<T> CriteriaLt<T>(params NHCriteria[] columns) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                for (int i = 0; i < columns.Length; i++)
                {
                    criteria.Add(Restrictions.Lt(columns[i].PropertyName, columns[i].Value));
                }
                return criteria.List<T>();
            }
        }
        /// <summary>
        /// 双参数验证，SQL语句为less than
        /// </summary>
        /// <typeparam name="T">需要验证的类型</typeparam>
        /// <param name="propertyName">参数名</param>
        /// <param name="otherPropertyName">另一个参数名</param>
        /// <returns>查询到的对象集合</returns>
        public virtual IList<T> CriteriaLtProperty<T>(string propertyName, string otherPropertyName) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                criteria.Add(Restrictions.LtProperty(propertyName, otherPropertyName));
                return criteria.List<T>();
            }
        }
        /// <summary>
        ///  双参数验证，SQL语句为not；
        ///  查询不符合参数的对象集合；
        /// </summary>
        /// <typeparam name="T">需要验证的类型</typeparam>
        /// <param name="propertyName">参数名</param>
        /// <param name="otherPropertyName">另一个参数名</param>
        /// <returns>查询到的对象集合</returns>
        public virtual IList<T> CriteriaNotEqProperty<T>(string propertyName, string otherPropertyName) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                criteria.Add(Restrictions.NotEqProperty(propertyName, otherPropertyName));
                return criteria.List<T>();
            }
        }
        /// <summary>
        /// Return the negation of an expression
        /// </summary>
        /// <typeparam name="T">需要验证的类型</typeparam>
        /// <param name="expression">nhibernate表达式</param>
        /// <returns>查询到的对象集合</returns>
        public virtual IList<T> CriteriaNot<T>(ICriterion expression) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                criteria.Add(Restrictions.Not(expression));
                return criteria.List<T>();
            }
        }
        #endregion

        #region Async
        /// <summary>
        /// 可覆写非空虚函数;
        /// 添加数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">具体数据</param>
        /// <returns>返回一个完整带有主键ID的对象</returns>
        public async virtual Task<T> InsertAsync<T>(T data) where T : class, new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    await session.SaveAsync(data);
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
        public async virtual Task<T> GetAsync<T>(object key)
            where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    Task<T> data = session.GetAsync<T>(key);
                    transaction.Commit();
                    return await data;
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
        public async virtual Task<T> CriteriaSelectAsync<T>(params NHCriteria[] columns)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                for (int i = 0; i < columns.Length; i++)
                {
                    criteria.Add(Restrictions.Eq(columns[i].PropertyName, columns[i].Value));
                }
                Task<T> data = criteria.UniqueResultAsync<T>();
                return await data;
            }
        }
        /// <summary>
        /// 条件查找符合的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async virtual Task<IList<T>> CriteriaSelectListAsync<T>(params NHCriteria[] columns)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                for (int i = 0; i < columns.Length; i++)
                {
                    criteria.Add(Restrictions.Eq(columns[i].PropertyName, columns[i].Value));
                }
                return await criteria.ListAsync<T>();
            }
        }
        /// <summary>
        /// 多条件验证，SQL语句为Equal
        /// </summary>
        /// <typeparam name="T">需要验证的类型</typeparam>
        /// <param name="columns">column类对象</param>
        /// <returns>验证是否成功</returns>
        public async virtual Task<bool> VerifyAsync<T>(params NHCriteria[] columns)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                for (int i = 0; i < columns.Length; i++)
                {
                    criteria.Add(Restrictions.Eq(columns[i].PropertyName, columns[i].Value));
                }
                Task<T> data = criteria.UniqueResultAsync<T>();
                return await data != null;
            }
        }
        /// <summary>
        /// 查找并返回所有条件对象的总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public async virtual Task<int> CountAsync<T>(params NHCriteria[] columns)
            where T : class
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria<T>();
                for (int i = 0; i < columns.Length; i++)
                {
                    criteria.Add(Restrictions.Eq(columns[i].PropertyName, columns[i].Value));
                }
                var resultList = await criteria.ListAsync<T>();
                return resultList.Count;
            }
        }
        /// <summary>
        /// 可覆写非空虚函数;
        /// 移除数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">具体数据</param>
        public async virtual Task DeleteAsync<T>(T data) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    await session.DeleteAsync(data);
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
        public async virtual Task UpdateAsync<T>(T data) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    await session.UpdateAsync(data);
                    transaction.Commit();
                }
            }
        }
        /// <summary>
        /// 异步保存或更新数据
        /// </summary>
        /// <typeparam name="T">无参构造的数据类型</typeparam>
        /// <param name="data">数据对象</param>
        public async virtual Task SaveOrUpdateAsync<T>(T data) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    await session.SaveOrUpdateAsync(data);
                    transaction.Commit();
                }
            }
        }
        /// <summary>
        /// 多条件验证，SQL语句为Like
        /// </summary>
        /// <typeparam name="T">需要验证的类型</typeparam>
        /// <param name="columns">column类对象</param>
        /// <returns>查询到的对象集合</returns>
        public async virtual Task<IList<T>> CriteriaLikeAsync<T>(params NHCriteria[] columns) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                for (int i = 0; i < columns.Length; i++)
                {
                    criteria.Add(Restrictions.Like(columns[i].PropertyName, columns[i].Value));
                }
                return await criteria.ListAsync<T>();
            }
        }
        /// <summary>
        /// 多条件验证，SQL语句为greater than
        /// </summary>
        /// <typeparam name="T">需要验证的类型</typeparam>
        /// <param name="columns">column类对象</param>
        /// <returns>查询到的对象集合</returns>
        public async virtual Task<IList<T>> CriteriaGtAsync<T>(params NHCriteria[] columns) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                for (int i = 0; i < columns.Length; i++)
                {
                    criteria.Add(Restrictions.Gt(columns[i].PropertyName, columns[i].Value));
                }
                return await criteria.ListAsync<T>();
            }
        }
        /// <summary>
        /// 多条件验证，SQL语句为less than
        /// </summary>
        /// <typeparam name="T">需要验证的类型</typeparam>
        /// <param name="columns">column类对象</param>
        /// <returns>查询到的对象集合</returns>
        public async virtual Task<IList<T>> CriteriaLtAsync<T>(params NHCriteria[] columns) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                for (int i = 0; i < columns.Length; i++)
                {
                    criteria.Add(Restrictions.Lt(columns[i].PropertyName, columns[i].Value));
                }
                return await criteria.ListAsync<T>();
            }
        }
        /// <summary>
        /// 双参数验证，SQL语句为less than
        /// </summary>
        /// <typeparam name="T">需要验证的类型</typeparam>
        /// <param name="propertyName">参数名</param>
        /// <param name="otherPropertyName">另一个参数名</param>
        /// <returns>查询到的对象集合</returns>
        public async virtual Task<IList<T>> CriteriaLtPropertyAsync<T>(string propertyName, string otherPropertyName) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                criteria.Add(Restrictions.LtProperty(propertyName, otherPropertyName));
                return await criteria.ListAsync<T>();
            }
        }
        /// <summary>
        ///  双参数验证，SQL语句为not；
        ///  查询不符合参数的对象集合；
        /// </summary>
        /// <typeparam name="T">需要验证的类型</typeparam>
        /// <param name="propertyName">参数名</param>
        /// <param name="otherPropertyName">另一个参数名</param>
        /// <returns>查询到的对象集合</returns>
        public async virtual Task<IList<T>> CriteriaNotEqPropertyAsync<T>(string propertyName, string otherPropertyName) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                criteria.Add(Restrictions.NotEqProperty(propertyName, otherPropertyName));
                return await criteria.ListAsync<T>();
            }
        }
        /// <summary>
        /// Return the negation of an expression
        /// </summary>
        /// <typeparam name="T">需要验证的类型</typeparam>
        /// <param name="expression">nhibernate表达式</param>
        /// <returns>查询到的对象集合</returns>
        public async virtual Task<IList<T>> CriteriaNotAsync<T>(ICriterion expression) where T : new()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(T));
                criteria.Add(Restrictions.Not(expression));
                return await criteria.ListAsync<T>();
            }
        }
        #endregion
    }

}
