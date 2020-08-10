using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using StackExchange.Redis;

namespace RedisDotNet
{
    public sealed partial  class RedisHelper
    {
        public sealed class Hash
        {
            #region Sync
            public static bool HashExist(string key, string dataKey)
            {
                key = AddRedisKeyPrefix(key);
                return redisDB.HashExists(key, dataKey);
            }
            /// <summary>
            /// 添加HashSet；
            /// 若key不存在，则创建新的；
            /// 若key存在，则覆写；
            /// </summary>
            /// <typeparam name="T">对象类型</typeparam>
            /// <param name="key">主Key</param>
            /// <param name="dataKey">数据Key</param>
            /// <param name="t">数据对象</param>
            /// <returns>是否添加成功</returns>
            public static bool HashSet<T>(string key, string dataKey, T t)
            {
                key = AddRedisKeyPrefix(key);
                string json = Utility.Json.ToJson(t);
                return redisDB.HashSet(key, dataKey, json);
            }
            /// <summary>
            /// 移除haseSet中的值
            /// </summary>
            /// <param name="key">主key</param>
            /// <param name="dataKey">数据Key</param>
            /// <returns>是否移除成功</returns>
            public static bool HashDelete(string key, string dataKey)
            {
                key = AddRedisKeyPrefix(key);
                return redisDB.HashDelete(key, dataKey);
            }
            public static long HashDelete(string key, params string[] dataKeys)
            {
                key = AddRedisKeyPrefix(key);
                var newValues = dataKeys.Select(o => (RedisValue)o).ToArray();
                return redisDB.HashDelete(key, newValues);
            }
            public static T HashGet<T>(string key, string dataKey)
            {
                key = AddRedisKeyPrefix(key);
                string value = redisDB.HashGet(key, dataKey);
                return Utility.Json.ToObject<T>(value);
            }
            public static double HashIncrement(string key, string dataKey, double value = 1)
            {
                key = AddRedisKeyPrefix(key);
                return redisDB.HashIncrement(key, dataKey, value);
            }
            /// <summary>
            /// 自减数字；
            /// 若数字存在，则在原基础上自减；
            /// 若不存在，则设置从0开始；
            /// </summary>
            /// <param name="key">主Key</param>
            /// <param name="dataKey">数据码</param>
            /// <param name="value">自减的量</param>
            /// <returns>减少后的数值</returns>
            public static double HashDecrement(string key, string dataKey, double value = 1)
            {
                key = AddRedisKeyPrefix(key);
                return redisDB.HashDecrement(key, dataKey, value);
            }
            public static string[] HashKeys(string key)
            {
                key = AddRedisKeyPrefix(key);
                RedisValue[] vars = redisDB.HashKeys(key);
                return vars.Select(d => d.ToString()).ToArray();
            }
            public static Dictionary<string,T>HashGetAll<T>(string key)
            {
                key = AddRedisKeyPrefix(key);
                var result = redisDB.HashGetAll(key);
                Dictionary<string, T> dict = new Dictionary<string, T>();
                foreach (var d in result)
                {
                    dict.Add(d.Name, Utility.Json.ToObject<T>(d.Value.ToString()));
                }
                return dict;
            }
            #endregion
            #region Async
            public async static Task<bool> HashExistAsync(string key,string dataKey)
            {
                key = AddRedisKeyPrefix(key);
                return await redisDB.HashExistsAsync(key, dataKey);
            }
            public async static Task<bool> HashSetAsync<T>(string key,string dataKey,T t)
            {
                key = AddRedisKeyPrefix(key);
                string json = Utility.Json.ToJson(t);
                return await redisDB.HashSetAsync(key, dataKey, json);
            }
            public async static Task<bool> HashDeleteAsync(string key,string dataKey)
            {
                key = AddRedisKeyPrefix(key);
                return await redisDB.HashDeleteAsync(key, dataKey);
            }
            public static async Task<long> HashDeleteAsync(string key,params string[] dataKeys)
            {
                key = AddRedisKeyPrefix(key);
                var newVars = dataKeys.Select(d => (RedisValue)d).ToArray();
                return await redisDB.HashDeleteAsync(key, newVars);
            }
            public async static Task<T>HashGetAsync<T>(string key,string dataKey)
            {
                key = AddRedisKeyPrefix(key);
                string result= await redisDB.HashGetAsync(key, dataKey);
                return Utility.Json.ToObject<T>(result);
            }
            public async static Task <double>HashIncrementAsync(string key,string dataKey,double value)
            {
                key = AddRedisKeyPrefix(key);
                return await redisDB.HashIncrementAsync(key, dataKey, value);
            }
            public async static Task<double>HashDecrementAsync(string key,string dataKey,double value)
            {
                key = AddRedisKeyPrefix(key);
                return await redisDB.HashDecrementAsync(key, dataKey, value);
            }
            public static async Task<string[]>HashKeysAsync(string key)
            {
                key = AddRedisKeyPrefix(key);
                RedisValue[] values = await redisDB.HashKeysAsync(key);
                return values.Select(d => d.ToString()).ToArray();
            }
            public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string key)
            {
                key = AddRedisKeyPrefix(key);
                var query = await redisDB.HashGetAllAsync(key);
                Dictionary<string, T> dict = new Dictionary<string, T>();
                foreach (var item in query)
                {
                    dict.Add(item.Name, Utility.Json.ToObject<T>(item.Value.ToString()));
                }
                return dict;
            }
            #endregion
        }

    }
}
