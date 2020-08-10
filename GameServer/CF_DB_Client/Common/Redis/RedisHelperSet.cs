using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cosmos;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;

namespace ProtocolCore
{
    public class RedisHelperSet 
    {
        /// <summary>
        /// 在Key集合中添加一个value值
        /// </summary>
        /// <param name="key">Key名称</param>
        /// <param name="jsonVar">值</param>
        /// <returns></returns>
        public static async Task<bool> SetAddAsync(string key, string jsonVar)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSet>(GRPCManager.Instance.Channel);
                var result = await remote.SetAddAsync(key, jsonVar);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 获取key集合值的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<long> SetLengthAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSet>(GRPCManager.Instance.Channel);
                var result = await remote.SetLengthAsync(key);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 判断Key集合中是否包含指定的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVar">要判断是值</param>
        /// <returns></returns>
        public static async Task<bool> SetContainsAsync(string key, string jsonVar)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSet>(GRPCManager.Instance.Channel);
                var result = await remote.SetContainsAsync(key,jsonVar);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 随机获取key集合中的一个值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<string> SetRandomMemberAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSet>(GRPCManager.Instance.Channel);
                var result = await remote.SetRandomMemberAsync(key);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 获取key所有值的集合
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<string[]> SetMembersAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSet>(GRPCManager.Instance.Channel);
                var result = await remote.SetMembersAsync(key);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 删除key集合中指定的value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVars"></param>
        /// <returns></returns>
        public static async Task<long> SetRemoveAsync(string key, params string[] jsonVars)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSet>(GRPCManager.Instance.Channel);
                var result = await remote.SetRemoveAsync(key, jsonVars);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 随机删除key集合中的一个值，并返回该值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<string> SetPopAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSet>(GRPCManager.Instance.Channel);
                var result = await remote.SetPopAsync(key);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 获取几个集合的并集
        /// </summary>
        /// <param name="keys">要操作的Key集合</param>
        /// <returns></returns>
        public static async Task<string[]> SetCombineUnionAsync(params string[] keys)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSet>(GRPCManager.Instance.Channel);
                var result = await remote.SetCombineUnionAsync(keys);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 获取几个集合的交集
        /// </summary>
        /// <param name="keys">要操作的Key集合</param>
        /// <returns></returns>
        public static async Task<string[]> SetCombineIntersectAsync(params string[] keys)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSet>(GRPCManager.Instance.Channel);
                var result = await remote.SetCombineIntersectAsync(keys);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 获取几个集合的差集
        /// </summary>
        /// <param name="keys">要操作的Key集合</param>
        /// <returns></returns>
        public static async Task<string[]> SetCombineDifferenceAsync(params string[] keys)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSet>(GRPCManager.Instance.Channel);
                var result = await remote.SetCombineDifferenceAsync(keys);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 获取几个集合的并集,并保存到一个新Key中
        /// </summary>
        /// <param name="destination">保存的新Key名称</param>
        /// <param name="keys">要操作的Key集合</param>
        /// <returns></returns>
        public static async Task<long> SetCombineUnionAndStoreAsync(string destination, params string[] keys)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSet>(GRPCManager.Instance.Channel);
                var result = await remote.SetCombineUnionAndStoreAsync(destination,keys);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 获取几个集合的交集,并保存到一个新Key中
        /// </summary>
        /// <param name="destination">保存的新Key名称</param>
        /// <param name="keys">要操作的Key集合</param>
        /// <returns></returns>
        public static async Task<long> SetCombineIntersectAndStoreAsync(string destination, params string[] keys)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSet>(GRPCManager.Instance.Channel);
                var result = await remote.SetCombineIntersectAndStoreAsync(destination, keys);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 获取几个集合的差集,并保存到一个新Key中
        /// </summary>
        /// <param name="destination">保存的新Key名称</param>
        /// <param name="keys">要操作的Key集合</param>
        /// <returns></returns>
        public static async Task<long> SetCombineDifferenceAndStoreAsync(string destination, params string[] keys)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSet>(GRPCManager.Instance.Channel);
                var result = await remote.SetCombineDifferenceAndStoreAsync(destination, keys);
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
