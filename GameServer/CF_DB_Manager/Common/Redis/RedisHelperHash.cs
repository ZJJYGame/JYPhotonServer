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
    public class RedisHelperHash 
    {

        public static async Task<bool> HashExistAsync(string key, string dataKey)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperHash>(GRPCManager.Instance.Channel);
                var result = await remote.HashExistAsync(key, dataKey);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        public static async Task<bool> HashSetAsync(string key, string dataKey, string jsonVar)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperHash>(GRPCManager.Instance.Channel);
                var result = await remote.HashSetAsync(key, dataKey,jsonVar);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        public static async Task<bool> HashDeleteAsync(string key, string dataKey)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperHash>(GRPCManager.Instance.Channel);
                var result = await remote.HashDeleteAsync(key, dataKey);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        public static async Task<long> HashsDeleteAsync(string key, params string[] dataKeys)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperHash>(GRPCManager.Instance.Channel);
                var result = await remote.HashsDeleteAsync(key, dataKeys);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        public static async Task<string> HashGetAsync(string key, string dataKey)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperHash>(GRPCManager.Instance.Channel);
                var result = await remote.HashGetAsync(key, dataKey);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        public static async Task<double> HashIncrementAsync(string key, string dataKey, double jsonVar)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperHash>(GRPCManager.Instance.Channel);
                var result = await remote.HashIncrementAsync(key, dataKey,jsonVar);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        public static async Task<double> HashDecrementAsync(string key, string dataKey, double jsonVar)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperHash>(GRPCManager.Instance.Channel);
                var result = await remote.HashDecrementAsync(key, dataKey, jsonVar);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        public static async Task<string[]> HashKeysAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperHash>(GRPCManager.Instance.Channel);
                var result = await remote.HashKeysAsync(key);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        public static async Task<Dictionary<string, string>> HashGetAllAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperHash>(GRPCManager.Instance.Channel);
                var result = await remote.HashGetAllAsync(key);
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
