using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using Cosmos;
namespace ProtocolCore
{
    public class RedisHelperList
    {

        /// <summary>
        /// 从左侧向list中添加多个值，返回集合总数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVars"></param>
        /// <returns></returns>
        public static async Task<long> ListLeftPushAsync(string key, string[] jsonVars)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperList>(GRPCManager.Instance.Channel);
                var result = await remote.ListLeftPushAsync(key, jsonVars);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 从右侧向list中添加多个值，返回集合总数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVars"></param>
        /// <returns></returns>
        public static async Task<long> ListRightPushAsync(string key, string[] jsonVars)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperList>(GRPCManager.Instance.Channel);
                var result = await remote.ListLeftPushAsync(key, jsonVars);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 从左侧向list中取出一个值并从list中删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<string> ListLeftPopAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperList>(GRPCManager.Instance.Channel);
                var result = await remote.ListLeftPopAsync(key);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 从右侧向list中取出一个值并从list中删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<string> ListRightPopAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperList>(GRPCManager.Instance.Channel);
                var result = await remote.ListRightPopAsync(key);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 从key的List中右侧取出一个值，并从左侧添加到destination集合中，且返回该数据对象
        /// </summary>
        /// <param name="key">要取出数据的List名称</param>
        /// <param name="destination">要添加到的List名称</param>
        /// <returns></returns>
        public static async Task<string> ListRightPopLeftPushAsync(string key, string destination)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperList>(GRPCManager.Instance.Channel);
                var result = await remote.ListRightPopLeftPushAsync(key,destination);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 在key的List指定值pivot之后插入value，返回集合总数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pivot">索引值</param>
        /// <param name="jsonVar">要插入的值</param>
        /// <returns></returns>
        public static async Task<long> ListInsertAfterAsync(string key, string pivot, string jsonVar)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperList>(GRPCManager.Instance.Channel);
                var result = await remote.ListInsertAfterAsync(key, pivot,jsonVar);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 在key的List指定值pivot之前插入value，返回集合总数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pivot">索引值</param>
        /// <param name="jsonVar">要插入的值</param>
        /// <returns></returns>
        public static async Task<long> ListInsertBeforeAsync(string key, string pivot, string jsonVar)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperList>(GRPCManager.Instance.Channel);
                var result = await remote.ListInsertBeforeAsync(key, pivot, jsonVar);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 从key的list中取出所有数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<string[]> ListRangeAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperList>(GRPCManager.Instance.Channel);
                var result = await remote.ListRangeAsync(key);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 从key的List获取指定索引的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static async Task<string> ListGetByIndexAsync(string key, long index)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperList>(GRPCManager.Instance.Channel);
                var result = await remote.ListGetByIndexAsync(key,index);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 获取key的list中数据个数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<long> ListLengthAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperList>(GRPCManager.Instance.Channel);
                var result = await remote.ListLengthAsync(key);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 从key的List中移除指定的值，返回删除个数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVar"></param>
        /// <returns></returns>
        public static async Task<long> ListRemoveAsync(string key, string jsonVar)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperList>(GRPCManager.Instance.Channel);
                var result = await remote.ListRemoveAsync(key,jsonVar);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
    }
}
