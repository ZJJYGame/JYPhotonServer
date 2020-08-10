using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using StackExchange.Redis;
namespace RedisDotNet
{
    public sealed partial class RedisHelper
    {
        public class Set
        {
            #region 同步方法
            /// <summary>
            /// 在Key集合中添加一个value值
            /// </summary>
            /// <typeparam name="T">数据类型</typeparam>
            /// <param name="key">Key名称</param>
            /// <param name="value">值</param>
            /// <returns></returns>
            public static bool SetAdd<T>(string key, T value)
            {
                key = AddRedisKeyPrefix(key);
                string jValue =  Utility.Json.ToJson(value);
                return redisDB.SetAdd(key, jValue);
            }
            /// <summary>
            /// 在Key集合中添加多个value值
            /// </summary>
            /// <typeparam name="T">数据类型</typeparam>
            /// <param name="key">Key名称</param>
            /// <param name="value">值列表</param>
            /// <returns></returns>
            public static long SetAdd<T>(string key, List<T> value)
            {
                key = AddRedisKeyPrefix(key);
                RedisValue[] valueList = ConvertRedisValue(value.ToArray());
                return redisDB.SetAdd(key, valueList);
            }
            /// <summary>
            /// 获取key集合值的数量
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public static long SetLength(string key)
            {
                key = AddRedisKeyPrefix(key);
                return redisDB.SetLength(key);
            }
            /// <summary>
            /// 判断Key集合中是否包含指定的值
            /// </summary>
            /// <typeparam name="T">值类型</typeparam>
            /// <param name="key"></param>
            /// <param name="value">要判断是值</param>
            /// <returns></returns>
            public static bool SetContains<T>(string key, T value)
            {
                key = AddRedisKeyPrefix(key);
                string jValue = Utility.Json.ToJson(value);
                return redisDB.SetContains(key, jValue);
            }
            /// <summary>
            /// 随机获取key集合中的一个值
            /// </summary>
            /// <typeparam name="T">数据类型</typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public static T SetRandomMember<T>(string key)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = redisDB.SetRandomMember(key);
                return Utility.Json.ToObject<T>(rValue.ToString());
            }
            /// <summary>
            /// 获取key所有值的集合
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public static List<T> SetMembers<T>(string key)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = redisDB.SetMembers(key);
                return ConvetList<T>(rValue);
            }
            /// <summary>
            /// 删除key集合中指定的value
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static long SetRemove<T>(string key, params T[] value)
            {
                key = AddRedisKeyPrefix(key);
                RedisValue[] valueList = ConvertRedisValue(value);
                return redisDB.SetRemove(key, valueList);
            }
            /// <summary>
            /// 随机删除key集合中的一个值，并返回该值
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public static T SetPop<T>(string key)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = redisDB.SetPop(key);
                return Utility.Json.ToObject<T>(rValue.ToString());
            }
            /// <summary>
            /// 获取几个集合的并集
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            public static List<T> SetCombineUnion<T>(params string[] keys)
            {
                return _SetCombine<T>(SetOperation.Union, keys);
            }
            /// <summary>
            /// 获取几个集合的交集
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            public static List<T> SetCombineIntersect<T>(params string[] keys)
            {
                return _SetCombine<T>(SetOperation.Intersect, keys);
            }
            /// <summary>
            /// 获取几个集合的差集
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            public static List<T> SetCombineDifference<T>(params string[] keys)
            {
                return _SetCombine<T>(SetOperation.Difference, keys);
            }

            /// <summary>
            /// 获取几个集合的并集,并保存到一个新Key中
            /// </summary>
            /// <param name="destination">保存的新Key名称</param>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            public static long SetCombineUnionAndStore(string destination, params string[] keys)
            {
                return _SetCombineAndStore(SetOperation.Union, destination, keys);
            }
            /// <summary>
            /// 获取几个集合的交集,并保存到一个新Key中
            /// </summary>
            /// <param name="destination">保存的新Key名称</param>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            public static long SetCombineIntersectAndStore(string destination, params string[] keys)
            {
                return _SetCombineAndStore(SetOperation.Intersect, destination, keys);
            }
            /// <summary>
            /// 获取几个集合的差集,并保存到一个新Key中
            /// </summary>
            /// <param name="destination">保存的新Key名称</param>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            public static long SetCombineDifferenceAndStore(string destination, params string[] keys)
            {
                return _SetCombineAndStore(SetOperation.Difference, destination, keys);
            }
            #endregion

            #region 异步方法
            /// <summary>
            /// 在Key集合中添加一个value值
            /// </summary>
            /// <typeparam name="T">数据类型</typeparam>
            /// <param name="key">Key名称</param>
            /// <param name="value">值</param>
            /// <returns></returns>
            public static async Task<bool> SetAddAsync<T>(string key, T value)
            {
                key = AddRedisKeyPrefix(key);
                string jValue = Utility.Json.ToJson(value);
                return await redisDB.SetAddAsync(key, jValue);
            }
            /// <summary>
            /// 在Key集合中添加多个value值
            /// </summary>
            /// <typeparam name="T">数据类型</typeparam>
            /// <param name="key">Key名称</param>
            /// <param name="value">值列表</param>
            /// <returns></returns>
            public static async Task<long> SetAddAsync<T>(string key, List<T> value)
            {
                key = AddRedisKeyPrefix(key);
                RedisValue[] valueList = ConvertRedisValue(value.ToArray());
                return await redisDB.SetAddAsync(key, valueList);
            }
            /// <summary>
            /// 获取key集合值的数量
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public static async Task<long> SetLengthAsync(string key)
            {
                key = AddRedisKeyPrefix(key);
                return await redisDB.SetLengthAsync(key);
            }
            /// <summary>
            /// 判断Key集合中是否包含指定的值
            /// </summary>
            /// <typeparam name="T">值类型</typeparam>
            /// <param name="key"></param>
            /// <param name="value">要判断是值</param>
            /// <returns></returns>
            public static async Task<bool> SetContainsAsync<T>(string key, T value)
            {
                key = AddRedisKeyPrefix(key);
                string jValue = Utility.Json.ToJson(value);
                return await redisDB.SetContainsAsync(key, jValue);
            }
            /// <summary>
            /// 随机获取key集合中的一个值
            /// </summary>
            /// <typeparam name="T">数据类型</typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public static async Task<T> SetRandomMemberAsync<T>(string key)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = await redisDB.SetRandomMemberAsync(key);
                return Utility.Json.ToObject<T>(rValue.ToString());
            }
            /// <summary>
            /// 获取key所有值的集合
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public static async Task<List<T>> SetMembersAsync<T>(string key)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = await redisDB.SetMembersAsync(key);
                return ConvetList<T>(rValue);
            }
            /// <summary>
            /// 删除key集合中指定的value
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static async Task<long> SetRemoveAsync<T>(string key, params T[] value)
            {
                key = AddRedisKeyPrefix(key);
                RedisValue[] valueList = ConvertRedisValue(value);
                return await redisDB.SetRemoveAsync(key, valueList);
            }
            /// <summary>
            /// 随机删除key集合中的一个值，并返回该值
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public static async Task<T> SetPopAsync<T>(string key)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = await redisDB.SetPopAsync(key);
                return Utility.Json.ToObject<T>(rValue.ToString());
            }
            /// <summary>
            /// 获取几个集合的并集
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            public static async Task<List<T>> SetCombineUnionAsync<T>(params string[] keys)
            {
                return await _SetCombineAsync<T>(SetOperation.Union, keys);
            }
            /// <summary>
            /// 获取几个集合的交集
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            public static async Task<List<T>> SetCombineIntersectAsync<T>(params string[] keys)
            {
                return await _SetCombineAsync<T>(SetOperation.Intersect, keys);
            }
            /// <summary>
            /// 获取几个集合的差集
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            public static async Task<List<T>> SetCombineDifferenceAsync<T>(params string[] keys)
            {
                return await _SetCombineAsync<T>(SetOperation.Difference, keys);
            }
            /// <summary>
            /// 获取几个集合的并集,并保存到一个新Key中
            /// </summary>
            /// <param name="destination">保存的新Key名称</param>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            public static async Task<long> SetCombineUnionAndStoreAsync(string destination, params string[] keys)
            {
                return await _SetCombineAndStoreAsync(SetOperation.Union, destination, keys);
            }
            /// <summary>
            /// 获取几个集合的交集,并保存到一个新Key中
            /// </summary>
            /// <param name="destination">保存的新Key名称</param>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            public static async Task<long> SetCombineIntersectAndStoreAsync(string destination, params string[] keys)
            {
                return await _SetCombineAndStoreAsync(SetOperation.Intersect, destination, keys);
            }
            /// <summary>
            /// 获取几个集合的差集,并保存到一个新Key中
            /// </summary>
            /// <param name="destination">保存的新Key名称</param>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            public static async Task<long> SetCombineDifferenceAndStoreAsync(string destination, params string[] keys)
            {
                return await _SetCombineAndStoreAsync(SetOperation.Difference, destination, keys);
            }

            #endregion

            #region 内部辅助方法
            /// <summary>
            /// 获取几个集合的交叉并集合
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="operation">Union：并集  Intersect：交集  Difference：差集  详见 <see cref="SetOperation"/></param>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            private static List<T> _SetCombine<T>(SetOperation operation, params string[] keys)
            {
                RedisKey[] keyList = ConvertRedisKeysAddSysCustomKey(keys);
                var rValue = redisDB.SetCombine(operation, keyList);
                return ConvetList<T>(rValue);
            }
            /// <summary>
            /// 获取几个集合的交叉并集合,并保存到一个新Key中
            /// </summary>
            /// <param name="operation">Union：并集  Intersect：交集  Difference：差集  详见 <see cref="SetOperation"/></param>
            /// <param name="destination">保存的新Key名称</param>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            private static long _SetCombineAndStore(SetOperation operation, string destination, params string[] keys)
            {
                destination = AddRedisKeyPrefix(destination);
                RedisKey[] keyList = ConvertRedisKeysAddSysCustomKey(keys);
                return redisDB.SetCombineAndStore(operation, destination, keyList);
            }
            /// <summary>
            /// 获取几个集合的交叉并集合
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="operation">Union：并集  Intersect：交集  Difference：差集  详见 <see cref="SetOperation"/></param>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            private static async Task<List<T>> _SetCombineAsync<T>(SetOperation operation, params string[] keys)
            {
                RedisKey[] keyList = ConvertRedisKeysAddSysCustomKey(keys);
                var rValue = await redisDB.SetCombineAsync(operation, keyList);
                return ConvetList<T>(rValue);
            }
            /// <summary>
            /// 获取几个集合的交叉并集合,并保存到一个新Key中
            /// </summary>
            /// <param name="operation">Union：并集  Intersect：交集  Difference：差集  详见 <see cref="SetOperation"/></param>
            /// <param name="destination">保存的新Key名称</param>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            private  static async Task<long> _SetCombineAndStoreAsync(SetOperation operation, string destination, params string[] keys)
            {
                destination = AddRedisKeyPrefix(destination);
                RedisKey[] keyList = ConvertRedisKeysAddSysCustomKey(keys);
                return await redisDB.SetCombineAndStoreAsync(operation, destination, keyList);
            }
            #endregion
        }

    }
}
