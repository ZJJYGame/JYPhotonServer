using System;
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
    public class RedisHelperString 
    {
        /// <summary>
        /// 异步方法 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public async UnaryResult<bool> StringSetAsync(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperString>(GRPCManager.Instance.Channel);
                var result = await remote.StringSetAsync(key, value,expiry);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 异步方法 添加多个key/value
        /// </summary>
        /// <param name="valueList">key/value集合</param>
        /// <returns></returns>
        public async UnaryResult<bool> StringSetsAsync(Dictionary<string, string> valueList)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperString>(GRPCManager.Instance.Channel);
                var result = await remote.StringSetsAsync(valueList);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 异步方法 在原有key的value值之后追加value
        /// </summary>
        /// <param name="key">追加的Key名称</param>
        /// <param name="value">追加的值</param>
        /// <returns></returns>
        public async UnaryResult<long> StringAppendAsync(string key, string value)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperString>(GRPCManager.Instance.Channel);
                var result = await remote.StringAppendAsync(key,value);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 异步方法 获取单个key的值
        /// </summary>
        /// <param name="key">要读取的Key名称</param>
        /// <returns></returns>
        public async UnaryResult<string> StringGetAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperString>(GRPCManager.Instance.Channel);
                var result = await remote.StringGetAsync(key);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 异步方法 获取多个key的value值
        /// </summary>
        /// <param name="keys">要获取值的Key集合</param>
        /// <returns></returns>
        public async UnaryResult<string[]> StringGetsAsync(params string[] keys)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperString>(GRPCManager.Instance.Channel);
                var result = await remote.StringGetsAsync(keys);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 异步方法 获取旧值赋上新值
        /// </summary>
        /// <param name="key">Key名称</param>
        /// <param name="value">新值</param>
        /// <returns></returns>
        public async UnaryResult<string> StringGetSetAsync(string key, string value)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperString>(GRPCManager.Instance.Channel);
                var result = await remote.StringGetSetAsync(key,value);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 异步方法 获取值的长度
        /// </summary>
        /// <param name="key">Key名称</param>
        /// <returns></returns>
        public async UnaryResult<long> StringGetLengthAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperString>(GRPCManager.Instance.Channel);
                var result = await remote.StringGetLengthAsync(key);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 异步方法 数字增长val，返回自增后的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public async UnaryResult<double> StringIncrementAsync(string key, double val = 1)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperString>(GRPCManager.Instance.Channel);
                var result = await remote.StringIncrementAsync(key,val);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 异步方法 数字减少val，返回自减少的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public async UnaryResult<double> StringDecrementAsync(string key, double val = 1)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperString>(GRPCManager.Instance.Channel);
                var result = await remote.StringDecrementAsync(key, val);
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
