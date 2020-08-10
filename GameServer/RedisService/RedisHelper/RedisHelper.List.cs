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
        public sealed class List
        {
            #region Sync
            /// <summary>
            /// 从左侧向list中添加一个值，返回集合总数
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static long ListLeftPush<T>(string key, T value)
            {
                key = AddRedisKeyPrefix(key);
                string json =Utility.Json.ToJson (value);
                return redisDB.ListLeftPush(key, json);
            }

            /// <summary>
            /// 从左侧向list中添加多个值，返回集合总数
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static long ListLeftPush<T>(string key, List<T> value)
            {
                key = AddRedisKeyPrefix(key);
                RedisValue[] valueList = ConvertRedisValue(value.ToArray());
                return redisDB.ListLeftPush(key, valueList);
            }

            /// <summary>
            /// 从右侧向list中添加一个值，返回集合总数
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static long ListRightPush<T>(string key, T value)
            {
                key = AddRedisKeyPrefix(key);
                string json= Utility.Json.ToJson(value);
                return redisDB.ListRightPush(key, json);
            }

            /// <summary>
            /// 从右侧向list中添加多个值，返回集合总数
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static long ListRightPush<T>(string key, List<T> value)
            {
                key = AddRedisKeyPrefix(key);
                RedisValue[] valueList = ConvertRedisValue(value.ToArray());
                return redisDB.ListRightPush(key, valueList);
            }

            /// <summary>
            /// 从左侧向list中取出一个值并从list中删除
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public static T ListLeftPop<T>(string key)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = redisDB.ListLeftPop(key);
                return Utility.Json.ToObject<T>(rValue.ToString());
            }

            /// <summary>
            /// 从右侧向list中取出一个值并从list中删除
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public static T ListRightPop<T>(string key)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = redisDB.ListRightPop(key);
                return Utility.Json.ToObject<T>(rValue.ToString());
            }

            /// <summary>
            /// 从key的List中右侧取出一个值，并从左侧添加到destination集合中，且返回该数据对象
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key">要取出数据的List名称</param>
            /// <param name="destination">要添加到的List名称</param>
            /// <returns></returns>
            public static T ListRightPopLeftPush<T>(string key, string destination)
            {
                key = AddRedisKeyPrefix(key);
                destination = AddRedisKeyPrefix(destination);
                var rValue = redisDB.ListRightPopLeftPush(key, destination);
                return Utility.Json.ToObject<T>(rValue.ToString());
            }
            /// <summary>
            /// 在key的List指定值pivot之后插入value，返回集合总数
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="pivot">索引值</param>
            /// <param name="value">要插入的值</param>
            /// <returns></returns>
            public static long ListInsertAfter<T>(string key, T pivot, T value)
            {
                key = AddRedisKeyPrefix(key);
                string pJson = Utility.Json.ToJson(pivot);
                string jJson = Utility.Json.ToJson(value);
                return redisDB.ListInsertAfter(key, pJson, jJson);
            }
            /// <summary>
            /// 在key的List指定值pivot之前插入value，返回集合总数
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="pivot">索引值</param>
            /// <param name="value">要插入的值</param>
            /// <returns></returns>
            public static long ListInsertBefore<T>(string key, T pivot, T value)
            {
                key = AddRedisKeyPrefix(key);
                string pValue = Utility.Json.ToJson(pivot);
                string jValue = Utility.Json.ToJson(value);
                return redisDB.ListInsertBefore(key, pValue, jValue);
            }
            /// <summary>
            /// 从key的list中取出所有数据
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public static List<T> ListRange<T>(string key)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = redisDB.ListRange(key);
                return ConvetList<T>(rValue);
            }
            /// <summary>
            /// 从key的List获取指定索引的值
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="index"></param>
            /// <returns></returns>
            public static T ListGetByIndex<T>(string key, long index)
            {
                key =AddRedisKeyPrefix(key);
                var rValue = redisDB.ListGetByIndex(key, index);
                return Utility.Json.ToObject<T>(rValue.ToString());
            }
            /// <summary>
            /// 获取key的list中数据个数
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public static long ListLength(string key)
            {
                key = AddRedisKeyPrefix(key);
                return redisDB.ListLength(key);
            }
            /// <summary>
            /// 从key的List中移除指定的值，返回删除个数
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static long ListRemove<T>(string key, T value)
            {
                key = AddRedisKeyPrefix(key);
                string jValue =Utility.Json.ToJson(value);
                return redisDB.ListRemove(key, jValue);
            }
            #endregion

            #region Async
            /// <summary>
            /// 从左侧向list中添加一个值，返回集合总数
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static async Task<long> ListLeftPushAsync<T>(string key, T value)
            {
                key = AddRedisKeyPrefix(key);
                string jValue =Utility.Json.ToJson(value);
                return await redisDB.ListLeftPushAsync(key, jValue);
            }
            /// <summary>
            /// 从左侧向list中添加多个值，返回集合总数
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static async Task<long> ListLeftPushAsync<T>(string key, List<T> value)
            {
                key = AddRedisKeyPrefix(key);
                RedisValue[] valueList = ConvertRedisValue(value.ToArray());
                return await redisDB.ListLeftPushAsync(key, valueList);
            }

            /// <summary>
            /// 从右侧向list中添加一个值，返回集合总数
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static async Task<long> ListRightPushAsync<T>(string key, T value)
            {
                key = AddRedisKeyPrefix(key);
                string jValue =Utility.Json.ToJson(value);
                return await redisDB.ListRightPushAsync(key, jValue);
            }
            /// <summary>
            /// 从右侧向list中添加多个值，返回集合总数
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static async Task<long> ListRightPushAsync<T>(string key, List<T> value)
            {
                key = AddRedisKeyPrefix(key);
                RedisValue[] valueList = ConvertRedisValue(value.ToArray());
                return await redisDB.ListRightPushAsync(key, valueList);
            }
            /// <summary>
            /// 从左侧向list中取出一个值并从list中删除
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public static async Task<T> ListLeftPopAsync<T>(string key)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = await redisDB.ListLeftPopAsync(key);
                return Utility.Json.ToObject<T>(rValue.ToString());
            }
            /// <summary>
            /// 从右侧向list中取出一个值并从list中删除
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public static async Task<T> ListRightPopAsync<T>(string key)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = await redisDB.ListRightPopAsync(key);
                return Utility.Json.ToObject<T>(rValue.ToString());
            }
            /// <summary>
            /// 从key的List中右侧取出一个值，并从左侧添加到destination集合中，且返回该数据对象
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key">要取出数据的List名称</param>
            /// <param name="destination">要添加到的List名称</param>
            /// <returns></returns>
            public static async Task<T> ListRightPopLeftPushAsync<T>(string key, string destination)
            {
                key = AddRedisKeyPrefix(key);
                destination = AddRedisKeyPrefix(destination);
                var rValue = await redisDB.ListRightPopLeftPushAsync(key, destination);
                return Utility.Json.ToObject<T>(rValue.ToString());
            }
            /// <summary>
            /// 在key的List指定值pivot之后插入value，返回集合总数
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="pivot">索引值</param>
            /// <param name="value">要插入的值</param>
            /// <returns></returns>
            public static async Task<long> ListInsertAfterAsync<T>(string key, T pivot, T value)
            {
                key = AddRedisKeyPrefix(key);
                string pValue =Utility.Json.ToJson(pivot);
                string jValue = Utility.Json.ToJson(value);
                return await redisDB.ListInsertAfterAsync(key, pValue, jValue);
            }
            /// <summary>
            /// 在key的List指定值pivot之前插入value，返回集合总数
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="pivot">索引值</param>
            /// <param name="value">要插入的值</param>
            /// <returns></returns>
            public static async Task<long> ListInsertBeforeAsync<T>(string key, T pivot, T value)
            {
                key = AddRedisKeyPrefix(key);
                string pValue = Utility.Json.ToJson(pivot);
                string jValue = Utility.Json.ToJson(value);
                return await redisDB.ListInsertBeforeAsync(key, pValue, jValue);
            }
            /// <summary>
            /// 从key的list中取出所有数据
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public static async Task<List<T>> ListRangeAsync<T>(string key)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = await redisDB.ListRangeAsync(key);
                return ConvetList<T>(rValue);
            }
            /// <summary>
            /// 从key的List获取指定索引的值
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="index"></param>
            /// <returns></returns>
            public static async Task<T> ListGetByIndexAsync<T>(string key, long index)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = await redisDB.ListGetByIndexAsync(key, index);
                return Utility.Json.ToObject<T>(rValue.ToString());
            }
            /// <summary>
            /// 获取key的list中数据个数
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public static async Task<long> ListLengthAsync(string key)
            {
                key = AddRedisKeyPrefix(key);
                return await redisDB.ListLengthAsync(key);
            }
            /// <summary>
            /// 从key的List中移除指定的值，返回删除个数
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static async Task<long> ListRemoveAsync<T>(string key, T value)
            {
                key = AddRedisKeyPrefix(key);
                string jValue = Utility.Json.ToJson(value);
                return await redisDB.ListRemoveAsync(key, jValue);
            }
            #endregion
        }
    }
}
