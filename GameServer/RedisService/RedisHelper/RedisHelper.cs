using System; 
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;
using Cosmos;
using System.Threading.Tasks;
using System.Linq;
using Cosmos;
namespace RedisDotNet
{
    public sealed partial  class RedisHelper
    {
         static readonly IDatabase redisDB; 
        static RedisHelper()
        {
           redisDB = RedisManager.Instance.RedisDB;
        }
        #region Sync
        /// <summary>
        /// 添加前缀
        /// </summary>
        /// <param name="key">添加前缀</param>
        /// <returns>添加前缀后的key</returns>
        public static string AddRedisKeyPrefix(string key)
        {
            return $"{RedisManager.RedisKeyPrefix}_{key}";
        }
        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="expiry">过期时间</param>
        /// <returns>是否设置成功</returns>
        public static bool KeyExpire(string key, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddRedisKeyPrefix(key);
            return redisDB.KeyExpire(key, expiry);
        }
        /// <summary>
        /// 移除key
        /// </summary>
        /// <param name="key">数据的key</param>
        /// <returns>是否移除成功</returns>
        public static bool KeyDelete(string key)
        {
            key = AddRedisKeyPrefix(key);
            return redisDB.KeyDelete(key);
        }
        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">key</param>
        /// <returns>删除成功的个数</returns>
        public static long KeyDelete(params string[] keys)
        {
            RedisKey[] newKeys = keys.Select(o => (RedisKey)AddRedisKeyPrefix(o)).ToArray();
            return redisDB.KeyDelete(newKeys);
        }
        #endregion

        #region Async
        /// <summary>
        /// 异步验证是否存在Key
        /// </summary>
        /// <param name="key">redisKey</param>
        /// <returns>异步的是否存在</returns>
        public static async Task<bool> KeyExistsAsync(string key)
        {
            key = AddRedisKeyPrefix(key);
            return await redisDB.KeyExistsAsync(key);
        }
        public static async Task<bool> KeyDeleteAsync(string key)
        {
            key = AddRedisKeyPrefix(key);
            return await redisDB.KeyDeleteAsync(key);
        }
        #endregion

        /// <summary>
        /// 将值反系列化成对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<T> ConvetList<T>(RedisValue[] values)
        {
            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = Utility.Json.ToObject<T>(item.ToString());
                result.Add(model);
            }
            return result;
        }
        /// <summary>
        /// 将string类型的Key转换成 <see cref="RedisKey"/> 型的Key
        /// </summary>
        /// <param name="redisKeys"></param>
        /// <returns></returns>
        public static RedisKey[] ConvertRedisKeys(List<string> redisKeys) => redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray();
        /// <summary>
        /// 将string类型的Key转换成 <see cref="RedisKey"/> 型的Key
        /// </summary>
        /// <param name="redisKeys"></param>
        /// <returns></returns>
        public static RedisKey[] ConvertRedisKeys(params string[] redisKeys) => redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray();
        /// <summary>
        /// 将string类型的Key转换成 <see cref="RedisKey"/> 型的Key，并添加前缀字符串
        /// </summary>
        /// <param name="redisKeys"></param>
        /// <returns></returns>
        public static RedisKey[] ConvertRedisKeysAddSysCustomKey(params string[] redisKeys) => redisKeys.Select(redisKey => (RedisKey)AddRedisKeyPrefix(redisKey)).ToArray();
        /// <summary>
        /// 将值集合转换成RedisValue集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisValues"></param>
        /// <returns></returns>
        public static RedisValue[] ConvertRedisValue<T>(params T[] redisValues) => redisValues.Select(d => (RedisValue)Utility.Json.ToJson(d)).ToArray();
    }
}
