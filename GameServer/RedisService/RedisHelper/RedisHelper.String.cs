using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using StackExchange.Redis;
namespace RedisDotNet
{
    public  sealed partial class  RedisHelper
    {

        public sealed class String
        {
            #region 同步方法
            /// <summary>
            /// 添加单个key value
            /// </summary>
            /// <param name="key">Redis Key</param>
            /// <param name="value">保存的值</param>
            /// <param name="expiry">过期时间</param>
            /// <returns></returns>
            public static bool StringSet(string key, string value, TimeSpan? expiry = default(TimeSpan?))
            {
                key = AddRedisKeyPrefix(key);
                return redisDB.StringSet(key, value, expiry);
            }
            /// <summary>
            /// 添加多个key/value
            /// </summary>
            /// <param name="valueList">key/value集合</param>
            /// <returns></returns>
            public static bool StringSet(Dictionary<string, string> valueList)
            {
                var newkeyValues = valueList.Select(p => new KeyValuePair<RedisKey, RedisValue>(AddRedisKeyPrefix(p.Key), p.Value)).ToArray();
                return redisDB.StringSet(newkeyValues);
            }
            /// <summary>
            /// 保存一个对象；
            /// 若值存在，则覆写； 
            /// </summary>
            /// <typeparam name="T">对象类型</typeparam>
            /// <param name="key">保存的Key名称</param>
            /// <param name="value">对象实体</param>
            /// <param name="expiry">过期时间</param>
            /// <returns></returns>
            public static bool StringSet<T>(string key, T value, TimeSpan? expiry = default(TimeSpan?))
            {
                key = AddRedisKeyPrefix(key);
                string jsonValue = Utility.Json.ToJson(value);
                return redisDB.StringSet(key, jsonValue, expiry);
            }
            /// <summary>
            /// 在原有key的value值之后追加value
            /// </summary>
            /// <param name="key">追加的Key名称</param>
            /// <param name="value">追加的值</param>
            /// <returns></returns>
            public static long StringAppend(string key, string value)
            {
                key = AddRedisKeyPrefix(key);
                return redisDB.StringAppend(key, value);
            }
            /// <summary>
            /// 获取单个key的值
            /// </summary>
            /// <param name="key">要读取的Key名称</param>
            /// <returns></returns>
            public static string StringGet(string key)
            {
                key = AddRedisKeyPrefix(key);
                return redisDB.StringGet(key);
            }
            /// <summary>
            /// 获取多个key的value值
            /// </summary>
            /// <param name="keys">要获取值的Key集合</param>
            /// <returns></returns>
            public static List<string> StringGet(params string[] keys)
            {
                var newKeys = ConvertRedisKeysAddSysCustomKey(keys);
                var values = redisDB.StringGet(newKeys);
                return values.Select(o => o.ToString()).ToList();
            }
            /// <summary>
            /// 获取单个key的value值
            /// </summary>
            /// <typeparam name="T">返回数据类型</typeparam>
            /// <param name="key">要获取值的Key集合</param>
            /// <returns></returns>
            public static T StringGet<T>(string key)
            {
                key = AddRedisKeyPrefix(key);
                var values = redisDB.StringGet(key);
                return Utility.Json.ToObject<T>(values.ToString());
            }
            /// <summary>
            /// 获取多个key的value值
            /// </summary>
            /// <typeparam name="T">返回数据类型</typeparam>
            /// <param name="keys">要获取值的Key集合</param>
            /// <returns></returns>
            public static List<T> StringGet<T>(params string[] keys)
            {
                var newKeys = ConvertRedisKeysAddSysCustomKey(keys);
                var values = redisDB.StringGet(newKeys);
                return ConvetList<T>(values);
            }
            /// <summary>
            /// 获取旧值赋上新值
            /// </summary>
            /// <param name="key">Key名称</param>
            /// <param name="value">新值</param>
            /// <returns></returns>
            public static string StringGetSet(string key, string value)
            {
                key = AddRedisKeyPrefix(key);
                return redisDB.StringGetSet(key, value);
            }
            /// <summary>
            /// 获取旧值赋上新值
            /// </summary>
            /// <typeparam name="T">数据类型</typeparam>
            /// <param name="key">Key名称</param>
            /// <param name="value">新值</param>
            /// <returns></returns>
            public static T StringGetSet<T>(string key, T value)
            {
                key = AddRedisKeyPrefix(key);
                string jsonValue = Utility.Json.ToJson(value);
                var oValue = redisDB.StringGetSet(key, jsonValue);
                return Utility.Json.ToObject<T>(oValue.ToString());
            }
            /// <summary>
            /// 获取值的长度
            /// </summary>
            /// <param name="key">Key名称</param>
            /// <returns></returns>
            public static long StringGetLength(string key)
            {
                key = AddRedisKeyPrefix(key);
                return redisDB.StringLength(key);
            }
            /// <summary>
            /// 数字增长val，返回自增后的值
            /// </summary>
            /// <param name="key"></param>
            /// <param name="val">可以为负</param>
            /// <returns>增长后的值</returns>
            public static double StringIncrement(string key, double val = 1)
            {
                key = AddRedisKeyPrefix(key);
                return redisDB.StringIncrement(key, val);
            }
            /// <summary>
            /// 数字减少val，返回自减少的值
            /// </summary>
            /// <param name="key"></param>
            /// <param name="val">可以为负</param>
            /// <returns>减少后的值</returns>
            public static double StringDecrement(string key, double val = 1)
            {
                key = AddRedisKeyPrefix(key);
                return redisDB.StringDecrement(key, val);
            }
            #endregion

            #region 异步方法
            /// <summary>
            /// 异步方法 保存单个key value
            /// </summary>
            /// <param name="key">Redis Key</param>
            /// <param name="value">保存的值</param>
            /// <param name="expiry">过期时间</param>
            /// <returns></returns>
            public static async Task<bool> StringSetAsync(string key, string value, TimeSpan? expiry = default(TimeSpan?))
            {
                key = AddRedisKeyPrefix(key);
                return await redisDB.StringSetAsync(key, value, expiry);
            }
            /// <summary>
            /// 异步方法 添加多个key/value
            /// </summary>
            /// <param name="valueList">key/value集合</param>
            /// <returns></returns>
            public static async Task<bool> StringSetAsync(Dictionary<string, string> valueList)
            {
                var newkeyValues = valueList.Select(p => new KeyValuePair<RedisKey, RedisValue>(AddRedisKeyPrefix(p.Key), p.Value)).ToArray();
                return await redisDB.StringSetAsync(newkeyValues);
            }
            /// <summary>
            /// 异步方法 保存一个对象
            /// </summary>
            /// <typeparam name="T">对象类型</typeparam>
            /// <param name="key">保存的Key名称</param>
            /// <param name="obj">对象实体</param>
            /// <param name="expiry">过期时间</param>
            /// <returns></returns>
            public static async Task<bool> StringSetAsync<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?))
            {
                key = AddRedisKeyPrefix(key);
                string jsonValue = Utility.Json.ToJson(obj);
                return await redisDB.StringSetAsync(key, jsonValue, expiry);
            }
            /// <summary>
            /// 异步方法 在原有key的value值之后追加value
            /// </summary>
            /// <param name="key">追加的Key名称</param>
            /// <param name="value">追加的值</param>
            /// <returns></returns>
            public static async Task<long> StringAppendAsync(string key, string value)
            {
                key = AddRedisKeyPrefix(key);
                return await redisDB.StringAppendAsync(key, value);
            }
            /// <summary>
            /// 异步方法 获取单个key的值
            /// </summary>
            /// <param name="key">要读取的Key名称</param>
            /// <returns></returns>
            public static async Task<string> StringGetAsync(string key)
            {
                key = AddRedisKeyPrefix(key);
                return await redisDB.StringGetAsync(key);
            }
            /// <summary>
            /// 异步方法 获取多个key的value值
            /// </summary>
            /// <param name="keys">要获取值的Key集合</param>
            /// <returns></returns>
            public static async Task<List<string>> StringGetAsync(params string[] keys)
            {
                var newKeys = ConvertRedisKeysAddSysCustomKey(keys);
                var values = await redisDB.StringGetAsync(newKeys);
                return values.Select(o => o.ToString()).ToList();
            }
            /// <summary>
            /// 异步方法 获取单个key的value值
            /// </summary>
            /// <typeparam name="T">返回数据类型</typeparam>
            /// <param name="key">要获取值的Key集合</param>
            /// <returns></returns>
            public static async Task<T> StringGetAsync<T>(string key)
            {
                key = AddRedisKeyPrefix(key);
                var values = await redisDB.StringGetAsync(key);
                return Utility.Json.ToObject<T>(values.ToString());
            }
            /// <summary>
            /// 异步方法 获取多个key的value值
            /// </summary>
            /// <typeparam name="T">返回数据类型</typeparam>
            /// <param name="keys">要获取值的Key集合</param>
            /// <returns></returns>
            public static async Task<List<T>> StringGetAsync<T>(params string[] keys)
            {
                var newKeys = ConvertRedisKeysAddSysCustomKey(keys);
                var values = await redisDB.StringGetAsync(newKeys);
                return ConvetList<T>(values);
            }
            /// <summary>
            /// 异步方法 获取旧值赋上新值
            /// </summary>
            /// <param name="key">Key名称</param>
            /// <param name="value">新值</param>
            /// <returns></returns>
            public static async Task<string> StringGetSetAsync(string key, string value)
            {
                key = AddRedisKeyPrefix(key);
                return await redisDB.StringGetSetAsync(key, value);
            }
            /// <summary>
            /// 异步方法 获取旧值赋上新值
            /// </summary>
            /// <typeparam name="T">数据类型</typeparam>
            /// <param name="key">Key名称</param>
            /// <param name="value">新值</param>
            /// <returns></returns>
            public static async Task<T> StringGetSetAsync<T>(string key, T value)
            {
                key = AddRedisKeyPrefix(key);
                string jsonValue = Utility.Json.ToJson(value);
                var oValue = await redisDB.StringGetSetAsync(key, jsonValue);
                return Utility.Json.ToObject<T>(oValue.ToString());
            }
            /// <summary>
            /// 异步方法 获取值的长度
            /// </summary>
            /// <param name="key">Key名称</param>
            /// <returns></returns>
            public static async Task<long> StringGetLengthAsync(string key)
            {
                key = AddRedisKeyPrefix(key);
                return await redisDB.StringLengthAsync(key);
            }
            /// <summary>
            /// 异步方法 数字增长val，返回自增后的值
            /// </summary>
            /// <param name="key"></param>
            /// <param name="val">可以为负</param>
            /// <returns>增长后的值</returns>
            public static async Task<double> StringIncrementAsync(string key, double val = 1)
            {
                key = AddRedisKeyPrefix(key);
                return await redisDB.StringIncrementAsync(key, val);
            }
            /// <summary>
            /// 异步方法 数字减少val，返回自减少的值
            /// </summary>
            /// <param name="key"></param>
            /// <param name="val">可以为负</param>
            /// <returns>减少后的值</returns>
            public static  async Task<double> StringDecrementAsync(string key, double val = 1)
            {
                key = AddRedisKeyPrefix(key);
                return await redisDB.StringDecrementAsync(key, val);
            }
            #endregion
        }
    }
}
