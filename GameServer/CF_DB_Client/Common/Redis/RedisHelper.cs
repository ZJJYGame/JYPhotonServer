using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using Cosmos;
using MessagePack;
using System;
namespace ProtocolCore
{
    public class RedisHelper
    {
        public static async Task<bool> KeyDeleteAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelper>(GRPCManager.Instance.Channel);
                var result = await remote.KeyDeleteAsync(key);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        public static async Task<long> KeysDeleteAsync(params string[] keys)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelper>(GRPCManager.Instance.Channel);
                var result = await remote.KeysDeleteAsync(keys);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        public static async Task<bool> KeyExistsAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelper>(GRPCManager.Instance.Channel);
                var result = await remote.KeyExistsAsync(key);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        public static async Task<bool> KeyExpireAsync(string key, TimeSpan? expiry = null)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelper>(GRPCManager.Instance.Channel);
                var result = await remote.KeyExpireAsync(key, expiry);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        public static async Task<object> KeyFulshAsync()
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelper>(GRPCManager.Instance.Channel);
                var result = await remote.KeyFulshAsync();
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }

        public static async Task<bool> KeyRenameAsync(string key, string newKey)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelper>(GRPCManager.Instance.Channel);
                var result = await remote.KeyRenameAsync(key, newKey);
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