using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cosmos;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
namespace ProtocolCore
{
    public class RedisHelperSortd
    {
        /// <summary>
        /// 添加一个值到Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVar"></param>
        /// <param name="score">排序分数，为空将获取集合中最大score加1</param>
        /// <returns></returns>
        public static async Task<bool> SortedSetAddAsync(string key, string jsonVar, double? score = null)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetAddAsync(key, jsonVar, score);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }

        /// <summary>
        /// 添加一个集合到Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVars"></param>
        /// <param name="score">排序分数，为空将获取集合中最大score加1</param>
        /// <returns></returns>
        public static async Task<long> SortedSetAddsAsync(string key, string[] jsonVars, double? score = null)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetAddsAsync(key, jsonVars, score);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<long> SortedSetLengthAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetLengthAsync(key);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }

        /// <summary>
        /// 获取指定起始值到结束值的集合数量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="startValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <returns></returns>
        public static async Task<long> SortedSetLengthByValueAsync(string key, string startValue, string endValue)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetLengthByValueAsync(key,startValue,endValue);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 获取指定Key的排序Score值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVar"></param>
        /// <returns></returns>
        public static async Task<double?> SortedSetScoreAsync(string key, string jsonVar)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetScoreAsync(key, jsonVar);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 获取指定Key中最小Score值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<double> SortedSetMinScoreAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetMinScoreAsync(key);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 获取指定Key中最大Score值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<double> SortedSetMaxScoreAsync(string key)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetMaxScoreAsync(key);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 删除Key中指定的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVars"></param>
        public static async Task<long> SortedSetRemoveAsync(string key, params string[] jsonVars)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetRemoveAsync(key,jsonVars);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }

        /// <summary>
        /// 删除指定起始值到结束值的数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="startValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <returns></returns>
        public static async Task<long> SortedSetRemoveRangeByValueAsync(string key, string startValue, string endValue)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetRemoveRangeByValueAsync(key, startValue,endValue);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 删除 从 start 开始的 stop 条数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public static async Task<long> SortedSetRemoveRangeByRankAsync(string key, long start, long stop)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetRemoveRangeByRankAsync(key, start, stop);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 根据排序分数Score，删除从 start 开始的 stop 条数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public static async Task<long> SortedSetRemoveRangeByScoreAsync(string key, double start, double stop)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetRemoveRangeByScoreAsync(key, start, stop);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 获取从 start 开始的 stop 条数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">起始数</param>
        /// <param name="stop">-1表示到结束，0为1条</param>
        /// <param name="desc">是否按降序排列</param>
        /// <returns></returns>
        public static async Task<string[]> SortedSetRangeByRankAsync(string key, long start = 0, long stop = -1, bool desc = false)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetRangeByRankAsync(key, start, stop,desc);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 获取从 start 开始的 stop 条数据包含Score，返回数据格式：Key=值，Value = Score
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">起始数</param>
        /// <param name="stop">-1表示到结束，0为1条</param>
        /// <param name="desc">是否按降序排列</param>
        /// <returns></returns>
        public static async Task<Dictionary<string, double>> SortedSetRangeByRankWithScoresAsync(string key, long start = 0, long stop = -1, bool desc = false)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetRangeByRankWithScoresAsync(key, start, stop, desc);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        ///  根据Score排序 获取从 start 开始的 stop 条数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">起始数</param>
        /// <param name="stop">-1表示到结束，0为1条</param>
        /// <param name="desc">是否按降序排列</param>
        /// <returns></returns>
        public static async Task<string[]> SortedSetRangeByScoreAsync(string key, double start = 0, double stop = -1, bool desc = false)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetRangeByScoreAsync(key, start, stop, desc);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 根据Score排序  获取从 start 开始的 stop 条数据包含Score，返回数据格式：Key=值，Value = Score
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">起始数</param>
        /// <param name="stop">-1表示到结束，0为1条</param>
        /// <param name="desc">是否按降序排列</param>
        /// <returns></returns>
        public static async Task<Dictionary<string, double>> SortedSetRangeByScoreWithScoresAsync(string key, double start = 0, double stop = -1, bool desc = false)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetRangeByScoreWithScoresAsync(key, start, stop, desc);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 获取指定起始值到结束值的数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="startValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <returns></returns>
        public static async Task<string[]> SortedSetRangeByValueAsync(string key, string startValue, string endValue)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetRangeByValueAsync(key, startValue,endValue);
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
        public static async Task<long> SortedSetCombineUnionAndStoreAsync(string destination, params string[] keys)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetCombineUnionAndStoreAsync(destination,keys);
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
        public static async Task<long> SortedSetCombineIntersectAndStoreAsync(string destination, params string[] keys)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetCombineIntersectAndStoreAsync(destination, keys);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 修改指定Key和值的Scores在原值上减去scores，并返回最终Scores
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVar"></param>
        /// <param name="scores"></param>
        /// <returns></returns>
        public static async Task<double> SortedSetDecrementAsync(string key, string jsonVar, double scores)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetDecrementAsync(key,jsonVar,scores);
                return await result;
            }
            catch (System.Exception e)
            {
                Utility.Debug.LogError(e.Message);
                return default;
            }
        }
        /// <summary>
        /// 修改指定Key和值的Scores在原值上增加scores，并返回最终Scores
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVar"></param>
        /// <param name="scores"></param>
        /// <returns></returns>
        public static async Task<double> SortedSetIncrementAsync(string key, string jsonVar, double scores)
        {
            try
            {
                var remote = MagicOnionClient.Create<IRedisHelperSorted>(GRPCManager.Instance.Channel);
                var result = await remote.SortedSetIncrementAsync(key, jsonVar, scores);
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
