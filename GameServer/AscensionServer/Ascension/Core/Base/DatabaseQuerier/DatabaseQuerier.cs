using RedisDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    public class DatabaseQuerier
    {
        #region StringRedis
        /// <summary>
        /// 获取一个对象；
        /// </summary>
        /// <typeparam name="T">存储的数据对象类型</typeparam>
        /// <param name="redisKey">redis存储的key</param>
        /// <param name="expiry">过期时间</param>
        /// <param name="columns">查询条件</param>
        /// <returns>查询到的数据对象</returns>
        public static T GetStringObject<T>(string redisKey, TimeSpan? expiry = default(TimeSpan?), params NHCriteria[] columns)
        {
            T data = default(T);
            var redisVar = RedisHelper.String.StringGet(redisKey);
            if (redisVar != null)
            {
                try
                {
                    data = Utility.Json.ToObject<T>(redisVar);
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError(e);
                }
            }
            else
            {
                data = NHibernateQuerier.CriteriaSelect<T>(columns);
                RedisHelper.String.StringSet(redisKey, data, expiry);
            }
            return data;
        }
        /// <summary>
        /// 插入数据到redis，并更新mysql
        /// </summary>
        /// <typeparam name="T">存储的数据对象类型</typeparam>
        /// <param name="redisKey">redis存储的key</param>
        /// <param name="data">存储的数据</param>
        /// <param name="expiry">过期时间</param>
        /// <returns>成功存入后的数据对象</returns>
        public static T InsertString<T>(string redisKey, T data, TimeSpan? expiry = default(TimeSpan?))
            where T : class, new()
        {
            var redisVar = RedisHelper.String.StringSet(redisKey, data, expiry);
            NHibernateQuerier.Insert(data);
            return data;
        }
        public static void DeleteString<T>(string redisKey, T data)
            where T : new()
        {
            if (RedisHelper.KeyDelete(redisKey))
            {
                NHibernateQuerier.Delete(data);
            }
        }
        /// <summary>
        /// 更新数据到redis，并更新mysql
        /// </summary>
        /// <typeparam name="T">存储的数据对象类型</typeparam>
        /// <param name="redisKey">redis存储的key</param>
        /// <param name="data">存储的数据</param>
        /// <param name="expiry">过期时间</param>
        public static void UpdateString<T>(string redisKey, T data, TimeSpan? expiry = default(TimeSpan?))
            where T : new()
        {
            RedisHelper.String.StringSet(redisKey, data, expiry);
            NHibernateQuerier.Update(data);
        }
        /// <summary>
        /// 获取一个对象的string数据；
        /// </summary>
        /// <typeparam name="T">存储的数据对象类型</typeparam>
        /// <param name="redisKey">redis存储的key</param>
        /// <param name="expiry">过期时间</param>
        /// <param name="columns">查询条件</param>
        /// <returns>获取到的数据string</returns>
        public static string GetString<T>(string redisKey, TimeSpan? expiry = default(TimeSpan?), params NHCriteria[] columns)
        {
            var redisVar = RedisHelper.String.StringGet(redisKey);
            if (redisVar == null)
            {
                var obj = NHibernateQuerier.CriteriaSelect<T>(columns);
                var data = Utility.Json.ToJson(obj);
                RedisHelper.String.StringSet(redisKey, data, expiry);
            }
            return redisVar;
        }
        #endregion
        #region Hash
        /// <summary>
        /// 存储一个hash数据
        /// </summary>
        /// <typeparam name="T">存储的数据对象类型</typeparam>
        /// <param name="key">redis主key</param>
        /// <param name="dataKey">rediskey对应的数据key</param>
        /// <param name="data">需要存储的数据对象</param>
        /// <returns>插入后的数据对象</returns>
        public static T InsertHash<T>(string key, string dataKey, T data) where T : class, new()
        {
            RedisHelper.Hash.HashSet(key, dataKey, data);
            return NHibernateQuerier.Insert(data);
        }
        /// <summary>
        /// 获取hash数据对象
        /// </summary>
        /// <typeparam name="T">存储的数据对象类型</typeparam>
        /// <param name="key">redis主key</param>
        /// <param name="dataKey">rediskey对应的数据key</param>
        /// <param name="columns">需要查询的colums</param>
        /// <returns>查询到的对象</returns>
        public static T GetHashObject<T>(string key, string dataKey, params NHCriteria[] columns)
            where T : class, new()
        {
            T obj = default(T);
            obj = RedisHelper.Hash.HashGet<T>(key, dataKey);
            if (obj == null)
            {
                obj = NHibernateQuerier.Get<T>(columns);
                if (obj != null)
                {
                    RedisHelper.Hash.HashSet(key, dataKey, obj);
                }
            }
            return obj;
        }
        public static string GetHash<T>(string key, string dataKey, params NHCriteria[] columns)
            where T : class, new()
        {
            T obj = default(T);
            obj = RedisHelper.Hash.HashGet<T>(key, dataKey);
            if (obj == null)
            {
                obj = NHibernateQuerier.Get<T>(columns);
                if (obj != null)
                {
                    RedisHelper.Hash.HashSet(key, dataKey, obj);
                }
            }
            return Utility.Json.ToJson(obj);
        }
        public static void SaveOrUpdateHash<T>(string key, string dataKey, T data) where T : class, new()
        {
            RedisHelper.Hash.HashSet(key, dataKey, data);
            NHibernateQuerier.SaveOrUpdate(data);
        }
        /// <summary>
        /// 移除一个hash
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">redis主key</param>
        /// <param name="dataKey">rediskey对应的数据key</param>
        /// <param name="data">需要删除的数据对象</param>
        public static void DeleteHash<T>(string key, string dataKey, T data)
            where T : class, new()
        {
            RedisHelper.Hash.HashDelete(key, dataKey);
            NHibernateQuerier.Delete(data);
        }
        /// <summary>
        /// 删除一组数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">redis主key</param>
        /// <param name="datas">myqsl中需要删除的数据条</param>
        /// <param name="dataKeys">rediskey对应的数据key</param>
        public static void DeleteHashs<T>(string key, T[] datas, params string[] dataKeys)
    where T : class, new()
        {
            RedisHelper.Hash.HashDelete(key, dataKeys);
            var length = datas.Length;
            for (int i = 0; i < length; i++)
            {
                NHibernateQuerier.Delete(datas[i]);
            }
        }
        #endregion

        #region Set

        #endregion
    }
}


