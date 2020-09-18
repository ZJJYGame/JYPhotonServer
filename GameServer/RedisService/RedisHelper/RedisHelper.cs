using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;
using Cosmos;
using System.Threading.Tasks;
using System.Linq;
namespace RedisDotNet
{
    public sealed partial class RedisHelper
    {
        static readonly IDatabase redisDB;
        static readonly IServer[] redisServers;
        static RedisHelper()
        {
            redisDB = RedisManager.Instance.RedisDB;
            redisServers = RedisManager.Instance.RedisServers;
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
        /// 非常消耗性能；
        /// 模糊查询K包含目标字段的key；
        /// order=0时，表示从最左边开始匹配；
        /// order=1时，表示从中间开始匹配；
        /// order=2时，表示从最右边开始匹配；
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="order">通配符位置</param>
        /// <param name="dbIndex">db的序号</param>
        /// <returns></returns>
        public static string[] FuzzyRedisKey(string keyword, byte order, int dbIndex = 0)
        {
            List<string> keys = new List<string>();
            string word = "";
            switch (order)
            {
                case 0:
                    word = $"*{keyword}";
                    break;
                case 1:
                    word = $"*{keyword}*";
                    break;
                case 2:
                    word = $"{keyword}*";
                    break;
            }
            if (string.IsNullOrEmpty(word))
                return null;
            foreach (var server in redisServers)
            {
                foreach (var key in server.Keys(dbIndex, word))
                {
                    keys.Add(key);
                }
            }
            return keys.ToArray();
        }
        /// <summary>
        /// 获取key剩下的剩余时间
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>剩余时间</returns>
        public static TimeSpan? KeyTimeToLive(string key)
        {
            key = AddRedisKeyPrefix(key);
            return redisDB.KeyTimeToLive(key);
        }
        /// <summary>
        /// 获取当前key被以及被存储的时长
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>时长</returns>
        public static TimeSpan? KeyIdleTime(string key)
        {
            key = AddRedisKeyPrefix(key);
            return redisDB.KeyIdleTime(key);
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
        /// 非常消耗性能；
        /// 模糊查询K包含目标字段的key；
        /// order=0时，表示从最左边开始匹配；
        /// order=1时，表示从中间开始匹配；
        /// order=2时，表示从最右边开始匹配；
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="order">通配符位置</param>
        /// <param name="dbIndex">db的序号</param>
        /// <returns></returns>
        public async static Task<string[]> FuzzyRedisKeyAsync(string keyword, byte order, int dbIndex = 0)
        {
            return await Task.Run(() =>
            {
                List<string> keys = new List<string>();
                string word = "";
                switch (order)
                {
                    case 0:
                        word = $"*{keyword}";
                        break;
                    case 1:
                        word = $"*{keyword}*";
                        break;
                    case 2:
                        word = $"{keyword}*";
                        break;
                }
                if (string.IsNullOrEmpty(word))
                    return null;
                foreach (var server in redisServers)
                {
                    foreach (var key in server.Keys(dbIndex, word))
                    {
                        keys.Add(key);
                    }
                }
                return keys.ToArray();
            });
        }
        /// <summary>
        /// 获取当前key被以及被存储的时长
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>时长</returns>
        public static async Task<TimeSpan?> KeyIdleTimeAsync(string key)
        {
            key = AddRedisKeyPrefix(key);
            return await redisDB.KeyIdleTimeAsync(key);
        }
        /// <summary>
        /// 获取key剩下的剩余时间
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>剩余时间</returns>
        public static async Task<TimeSpan?> KeyTimeToLiveAsync(string key)
        {
            key = AddRedisKeyPrefix(key);
            return await redisDB.KeyTimeToLiveAsync(key);
        }
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
