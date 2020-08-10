using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace RedisDotNet
{
    public  sealed partial class RedisHelper
    {
        public sealed class Sorted
        {

            #region 同步方法

            /// <summary>
            /// 添加一个值到Key
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="score">排序分数，为空将获取集合中最大score加1</param>
            /// <returns></returns>
            public bool SortedSetAdd<T>(string key, T value, double? score = null)
            {
                key = AddRedisKeyPrefix(key);
                double scoreNum = score ?? _GetScore(key);
                return redisDB.SortedSetAdd(key, Utility.Json.ToJson(value), scoreNum);
            }

            /// <summary>
            /// 添加一个集合到Key
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="score">排序分数，为空将获取集合中最大score加1</param>
            /// <returns></returns>
            public long SortedSetAdd<T>(string key, List<T> value, double? score = null)
            {
                key = AddRedisKeyPrefix(key);
                double scoreNum = score ?? _GetScore(key);
                SortedSetEntry[] rValue = value.Select(o => new SortedSetEntry(Utility.Json.ToJson(o), scoreNum++)).ToArray();
                return redisDB.SortedSetAdd(key, rValue);
            }

            /// <summary>
            /// 获取集合中的数量
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public long SortedSetLength(string key)
            {
                key = AddRedisKeyPrefix(key);
                return redisDB.SortedSetLength(key);
            }

            /// <summary>
            /// 获取指定起始值到结束值的集合数量
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="startValue">起始值</param>
            /// <param name="endValue">结束值</param>
            /// <returns></returns>
            public long SortedSetLengthByValue<T>(string key, T startValue, T endValue)
            {
                key = AddRedisKeyPrefix(key);
                var sValue = Utility.Json.ToJson(startValue);
                var eValue = Utility.Json.ToJson(endValue);
                return redisDB.SortedSetLengthByValue(key, sValue, eValue);
            }

            /// <summary>
            /// 获取指定Key的排序Score值
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public double? SortedSetScore<T>(string key, T value)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = Utility.Json.ToJson(value);
                return redisDB.SortedSetScore(key, rValue);
            }

            /// <summary>
            /// 获取指定Key中最小Score值
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public double SortedSetMinScore(string key)
            {
                key = AddRedisKeyPrefix(key);
                double dValue = 0;
                var rValue = redisDB.SortedSetRangeByRankWithScores(key, 0, 0, Order.Ascending).FirstOrDefault();
                dValue = rValue != null ? rValue.Score : 0;
                return dValue;
            }

            /// <summary>
            /// 获取指定Key中最大Score值
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public double SortedSetMaxScore(string key)
            {
                key = AddRedisKeyPrefix(key);
                double dValue = 0;
                var rValue = redisDB.SortedSetRangeByRankWithScores(key, 0, 0, Order.Descending).FirstOrDefault();
                dValue = rValue != null ? rValue.Score : 0;
                return dValue;
            }

            /// <summary>
            /// 删除Key中指定的值
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public long SortedSetRemove<T>(string key, params T[] value)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = ConvertRedisValue<T>(value);
                return redisDB.SortedSetRemove(key, rValue);
            }

            /// <summary>
            /// 删除指定起始值到结束值的数据
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="startValue">起始值</param>
            /// <param name="endValue">结束值</param>
            /// <returns></returns>
            public long SortedSetRemoveRangeByValue<T>(string key, T startValue, T endValue)
            {
                key = AddRedisKeyPrefix(key);
                var sValue = Utility.Json.ToJson(startValue);
                var eValue = Utility.Json.ToJson(endValue);
                return redisDB.SortedSetRemoveRangeByValue(key, sValue, eValue);
            }

            /// <summary>
            /// 删除 从 start 开始的 stop 条数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="start"></param>
            /// <param name="stop"></param>
            /// <returns></returns>
            public long SortedSetRemoveRangeByRank(string key, long start, long stop)
            {
                key = AddRedisKeyPrefix(key);
                return redisDB.SortedSetRemoveRangeByRank(key, start, stop);
            }

            /// <summary>
            /// 根据排序分数Score，删除从 start 开始的 stop 条数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="start"></param>
            /// <param name="stop"></param>
            /// <returns></returns>
            public long SortedSetRemoveRangeByScore(string key, double start, double stop)
            {
                key = AddRedisKeyPrefix(key);
                return redisDB.SortedSetRemoveRangeByScore(key, start, stop);
            }

            /// <summary>
            /// 获取从 start 开始的 stop 条数据
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="start">起始数</param>
            /// <param name="stop">-1表示到结束，0为1条</param>
            /// <param name="desc">是否按降序排列</param>
            /// <returns></returns>
            public List<T> SortedSetRangeByRank<T>(string key, long start = 0, long stop = -1, bool desc = false)
            {
                key = AddRedisKeyPrefix(key);
                Order orderBy = desc ? Order.Descending : Order.Ascending;
                var rValue = redisDB.SortedSetRangeByRank(key, start, stop, orderBy);
                return ConvetList<T>(rValue);
            }

            /// <summary>
            /// 获取从 start 开始的 stop 条数据包含Score，返回数据格式：Key=值，Value = Score
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="start">起始数</param>
            /// <param name="stop">-1表示到结束，0为1条</param>
            /// <param name="desc">是否按降序排列</param>
            /// <returns></returns>
            public Dictionary<T, double> SortedSetRangeByRankWithScores<T>(string key, long start = 0, long stop = -1, bool desc = false)
            {
                key = AddRedisKeyPrefix(key);
                Order orderBy = desc ? Order.Descending : Order.Ascending;
                var rValue = redisDB.SortedSetRangeByRankWithScores(key, start, stop, orderBy);
                Dictionary<T, double> dicList = new Dictionary<T, double>();
                foreach (var item in rValue)
                {
                    dicList.Add(Utility.Json.ToObject<T>(item.Element.ToString()), item.Score);
                }
                return dicList;
            }

            /// <summary>
            ///  根据Score排序 获取从 start 开始的 stop 条数据
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="start">起始数</param>
            /// <param name="stop">-1表示到结束，0为1条</param>
            /// <param name="desc">是否按降序排列</param>
            /// <returns></returns>
            public List<T> SortedSetRangeByScore<T>(string key, double start = 0, double stop = -1, bool desc = false)
            {
                key = AddRedisKeyPrefix(key);
                Order orderBy = desc ? Order.Descending : Order.Ascending;
                var rValue = redisDB.SortedSetRangeByScore(key, start, stop, Exclude.None, orderBy);
                return ConvetList<T>(rValue);
            }

            /// <summary>
            /// 根据Score排序  获取从 start 开始的 stop 条数据包含Score，返回数据格式：Key=值，Value = Score
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="start">起始数</param>
            /// <param name="stop">-1表示到结束，0为1条</param>
            /// <param name="desc">是否按降序排列</param>
            /// <returns></returns>
            public Dictionary<T, double> SortedSetRangeByScoreWithScores<T>(string key, double start = 0, double stop = -1, bool desc = false)
            {
                key = AddRedisKeyPrefix(key);
                Order orderBy = desc ? Order.Descending : Order.Ascending;
                var rValue = redisDB.SortedSetRangeByScoreWithScores(key, start, stop, Exclude.None, orderBy);
                Dictionary<T, double> dicList = new Dictionary<T, double>();
                foreach (var item in rValue)
                {
                    dicList.Add(Utility.Json.ToObject<T>(item.Element.ToString()), item.Score);
                }
                return dicList;
            }

            /// <summary>
            /// 获取指定起始值到结束值的数据
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="startValue">起始值</param>
            /// <param name="endValue">结束值</param>
            /// <returns></returns>
            public List<T> SortedSetRangeByValue<T>(string key, T startValue, T endValue)
            {
                key = AddRedisKeyPrefix(key);
                var sValue = Utility.Json.ToJson(startValue);
                var eValue = Utility.Json.ToJson(endValue);
                var rValue = redisDB.SortedSetRangeByValue(key, sValue, eValue);
                return ConvetList<T>(rValue);
            }

            /// <summary>
            /// 获取几个集合的并集,并保存到一个新Key中
            /// </summary>
            /// <param name="destination">保存的新Key名称</param>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            public long SortedSetCombineUnionAndStore(string destination, params string[] keys)
            {
                return _SortedSetCombineAndStore(SetOperation.Union, destination, keys);
            }

            /// <summary>
            /// 获取几个集合的交集,并保存到一个新Key中
            /// </summary>
            /// <param name="destination">保存的新Key名称</param>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            public long SortedSetCombineIntersectAndStore(string destination, params string[] keys)
            {
                return _SortedSetCombineAndStore(SetOperation.Intersect, destination, keys);
            }


            //交集似乎并不支持
            ///// <summary>
            ///// 获取几个集合的差集,并保存到一个新Key中
            ///// </summary>
            ///// <param name="destination">保存的新Key名称</param>
            ///// <param name="keys">要操作的Key集合</param>
            ///// <returns></returns>
            //public long SortedSetCombineDifferenceAndStore(string destination, params string[] keys)
            //{
            //    return _SortedSetCombineAndStore(SetOperation.Difference, destination, keys);
            //}



            /// <summary>
            /// 修改指定Key和值的Scores在原值上减去scores，并返回最终Scores
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="scores"></param>
            /// <returns></returns>
            public double SortedSetDecrement<T>(string key, T value, double scores)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = Utility.Json.ToJson(value);
                return redisDB.SortedSetDecrement(key, rValue, scores);
            }

            /// <summary>
            /// 修改指定Key和值的Scores在原值上增加scores，并返回最终Scores
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="scores"></param>
            /// <returns></returns>
            public double SortedSetIncrement<T>(string key, T value, double scores)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = Utility.Json.ToJson(value);
                return redisDB.SortedSetIncrement(key, rValue, scores);
            }



            #endregion

            #region 异步方法

            /// <summary>
            /// 添加一个值到Key
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="score">排序分数，为空将获取集合中最大score加1</param>
            /// <returns></returns>
            public async Task<bool> SortedSetAddAsync<T>(string key, T value, double? score = null)
            {
                key = AddRedisKeyPrefix(key);
                double scoreNum = score ?? _GetScore(key);
                return await redisDB.SortedSetAddAsync(key, Utility.Json.ToJson(value), scoreNum);
            }

            /// <summary>
            /// 添加一个集合到Key
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="score">排序分数，为空将获取集合中最大score加1</param>
            /// <returns></returns>
            public async Task<long> SortedSetAddAsync<T>(string key, List<T> value, double? score = null)
            {
                key = AddRedisKeyPrefix(key);
                double scoreNum = score ?? _GetScore(key);
                SortedSetEntry[] rValue = value.Select(o => new SortedSetEntry(Utility.Json.ToJson(o), scoreNum++)).ToArray();
                return await redisDB.SortedSetAddAsync(key, rValue);
            }

            /// <summary>
            /// 获取集合中的数量
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public async Task<long> SortedSetLengthAsync(string key)
            {
                key = AddRedisKeyPrefix(key);
                return await redisDB.SortedSetLengthAsync(key);
            }

            /// <summary>
            /// 获取指定起始值到结束值的集合数量
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="startValue">起始值</param>
            /// <param name="endValue">结束值</param>
            /// <returns></returns>
            public async Task<long> SortedSetLengthByValueAsync<T>(string key, T startValue, T endValue)
            {
                key = AddRedisKeyPrefix(key);
                var sValue = Utility.Json.ToJson(startValue);
                var eValue = Utility.Json.ToJson(endValue);
                return await redisDB.SortedSetLengthByValueAsync(key, sValue, eValue);
            }

            /// <summary>
            /// 获取指定Key的排序Score值
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public async Task<double?> SortedSetScoreAsync<T>(string key, T value)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = Utility.Json.ToJson(value);
                return await redisDB.SortedSetScoreAsync(key, rValue);
            }

            /// <summary>
            /// 获取指定Key中最小Score值
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public async Task<double> SortedSetMinScoreAsync(string key)
            {
                key = AddRedisKeyPrefix(key);
                double dValue = 0;
                var rValue = (await redisDB.SortedSetRangeByRankWithScoresAsync(key, 0, 0, Order.Ascending)).FirstOrDefault();
                dValue = rValue != null ? rValue.Score : 0;
                return dValue;
            }

            /// <summary>
            /// 获取指定Key中最大Score值
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public async Task<double> SortedSetMaxScoreAsync(string key)
            {
                key = AddRedisKeyPrefix(key);
                double dValue = 0;
                var rValue = (await redisDB.SortedSetRangeByRankWithScoresAsync(key, 0, 0, Order.Descending)).FirstOrDefault();
                dValue = rValue != null ? rValue.Score : 0;
                return dValue;
            }

            /// <summary>
            /// 删除Key中指定的值
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public async Task<long> SortedSetRemoveAsync<T>(string key, params T[] value)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = ConvertRedisValue<T>(value);
                return await redisDB.SortedSetRemoveAsync(key, rValue);
            }

            /// <summary>
            /// 删除指定起始值到结束值的数据
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="startValue">起始值</param>
            /// <param name="endValue">结束值</param>
            /// <returns></returns>
            public async Task<long> SortedSetRemoveRangeByValueAsync<T>(string key, T startValue, T endValue)
            {
                key = AddRedisKeyPrefix(key);
                var sValue = Utility.Json.ToJson(startValue);
                var eValue = Utility.Json.ToJson(endValue);
                return await redisDB.SortedSetRemoveRangeByValueAsync(key, sValue, eValue);
            }

            /// <summary>
            /// 删除 从 start 开始的 stop 条数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="start"></param>
            /// <param name="stop"></param>
            /// <returns></returns>
            public async Task<long> SortedSetRemoveRangeByRankAsync(string key, long start, long stop)
            {
                key = AddRedisKeyPrefix(key);
                return await redisDB.SortedSetRemoveRangeByRankAsync(key, start, stop);
            }

            /// <summary>
            /// 根据排序分数Score，删除从 start 开始的 stop 条数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="start"></param>
            /// <param name="stop"></param>
            /// <returns></returns>
            public async Task<long> SortedSetRemoveRangeByScoreAsync(string key, double start, double stop)
            {
                key = AddRedisKeyPrefix(key);
                return await redisDB.SortedSetRemoveRangeByScoreAsync(key, start, stop);
            }

            /// <summary>
            /// 获取从 start 开始的 stop 条数据
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="start">起始数</param>
            /// <param name="stop">-1表示到结束，0为1条</param>
            /// <param name="desc">是否按降序排列</param>
            /// <returns></returns>
            public async Task<List<T>> SortedSetRangeByRankAsync<T>(string key, long start = 0, long stop = -1, bool desc = false)
            {
                key = AddRedisKeyPrefix(key);
                Order orderBy = desc ? Order.Descending : Order.Ascending;
                var rValue = await redisDB.SortedSetRangeByRankAsync(key, start, stop, orderBy);
                return ConvetList<T>(rValue);
            }

            /// <summary>
            /// 获取从 start 开始的 stop 条数据包含Score，返回数据格式：Key=值，Value = Score
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="start">起始数</param>
            /// <param name="stop">-1表示到结束，0为1条</param>
            /// <param name="desc">是否按降序排列</param>
            /// <returns></returns>
            public async Task<Dictionary<T, double>> SortedSetRangeByRankWithScoresAsync<T>(string key, long start = 0, long stop = -1, bool desc = false)
            {
                key = AddRedisKeyPrefix(key);
                Order orderBy = desc ? Order.Descending : Order.Ascending;
                var rValue = await redisDB.SortedSetRangeByRankWithScoresAsync(key, start, stop, orderBy);
                Dictionary<T, double> dicList = new Dictionary<T, double>();
                foreach (var item in rValue)
                {
                    dicList.Add(Utility.Json.ToObject<T>(item.Element.ToString()), item.Score);
                }
                return dicList;
            }

            /// <summary>
            ///  根据Score排序 获取从 start 开始的 stop 条数据
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="start">起始数</param>
            /// <param name="stop">-1表示到结束，0为1条</param>
            /// <param name="desc">是否按降序排列</param>
            /// <returns></returns>
            public async Task<List<T>> SortedSetRangeByScoreAsync<T>(string key, double start = 0, double stop = -1, bool desc = false)
            {
                key = AddRedisKeyPrefix(key);
                Order orderBy = desc ? Order.Descending : Order.Ascending;
                var rValue = await redisDB.SortedSetRangeByScoreAsync(key, start, stop, Exclude.None, orderBy);
                return ConvetList<T>(rValue);
            }

            /// <summary>
            /// 根据Score排序  获取从 start 开始的 stop 条数据包含Score，返回数据格式：Key=值，Value = Score
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="start">起始数</param>
            /// <param name="stop">-1表示到结束，0为1条</param>
            /// <param name="desc">是否按降序排列</param>
            /// <returns></returns>
            public async Task<Dictionary<T, double>> SortedSetRangeByScoreWithScoresAsync<T>(string key, double start = 0, double stop = -1, bool desc = false)
            {
                key = AddRedisKeyPrefix(key);
                Order orderBy = desc ? Order.Descending : Order.Ascending;
                var rValue = await redisDB.SortedSetRangeByScoreWithScoresAsync(key, start, stop, Exclude.None, orderBy);
                Dictionary<T, double> dicList = new Dictionary<T, double>();
                foreach (var item in rValue)
                {
                    dicList.Add(Utility.Json.ToObject<T>(item.Element.ToString()), item.Score);
                }
                return dicList;
            }

            /// <summary>
            /// 获取指定起始值到结束值的数据
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="startValue">起始值</param>
            /// <param name="endValue">结束值</param>
            /// <returns></returns>
            public async Task<List<T>> SortedSetRangeByValueAsync<T>(string key, T startValue, T endValue)
            {
                key = AddRedisKeyPrefix(key);
                var sValue = Utility.Json.ToJson(startValue);
                var eValue = Utility.Json.ToJson(endValue);
                var rValue = await redisDB.SortedSetRangeByValueAsync(key, sValue, eValue);
                return ConvetList<T>(rValue);
            }

            /// <summary>
            /// 获取几个集合的并集,并保存到一个新Key中
            /// </summary>
            /// <param name="destination">保存的新Key名称</param>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            public async Task<long> SortedSetCombineUnionAndStoreAsync(string destination, params string[] keys)
            {
                return await _SortedSetCombineAndStoreAsync(SetOperation.Union, destination, keys);
            }

            /// <summary>
            /// 获取几个集合的交集,并保存到一个新Key中
            /// </summary>
            /// <param name="destination">保存的新Key名称</param>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            public async Task<long> SortedSetCombineIntersectAndStoreAsync(string destination, params string[] keys)
            {
                return await _SortedSetCombineAndStoreAsync(SetOperation.Intersect, destination, keys);
            }

            ///// <summary>
            ///// 获取几个集合的差集,并保存到一个新Key中
            ///// </summary>
            ///// <param name="destination">保存的新Key名称</param>
            ///// <param name="keys">要操作的Key集合</param>
            ///// <returns></returns>
            //public async Task<long> SortedSetCombineDifferenceAndStoreAsync(string destination, params string[] keys)
            //{
            //    return await _SortedSetCombineAndStoreAsync(SetOperation.Difference, destination, keys);
            //}

            /// <summary>
            /// 修改指定Key和值的Scores在原值上减去scores，并返回最终Scores
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="scores"></param>
            /// <returns></returns>
            public async Task<double> SortedSetDecrementAsync<T>(string key, T value, double scores)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = Utility.Json.ToJson(value);
                return await redisDB.SortedSetDecrementAsync(key, rValue, scores);
            }

            /// <summary>
            /// 修改指定Key和值的Scores在原值上增加scores，并返回最终Scores
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="scores"></param>
            /// <returns></returns>
            public async Task<double> SortedSetIncrementAsync<T>(string key, T value, double scores)
            {
                key = AddRedisKeyPrefix(key);
                var rValue = Utility.Json.ToJson(value);
                return await redisDB.SortedSetIncrementAsync(key, rValue, scores);
            }



            #endregion

            #region 内部辅助方法
            /// <summary>
            /// 获取指定Key中最大Score值,
            /// </summary>
            /// <param name="key">key名称，注意要先添加上Key前缀</param>
            /// <returns></returns>
            private double _GetScore(string key)
            {
                double dValue = 0;
                var rValue = redisDB.SortedSetRangeByRankWithScores(key, 0, 0, Order.Descending).FirstOrDefault();
                dValue = rValue != null ? rValue.Score : 0;
                return dValue + 1;
            }

            /// <summary>
            /// 获取几个集合的交叉并集合,并保存到一个新Key中
            /// </summary>
            /// <param name="operation">Union：并集  Intersect：交集  Difference：差集  详见 <see cref="SetOperation"/></param>
            /// <param name="destination">保存的新Key名称</param>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            private long _SortedSetCombineAndStore(SetOperation operation, string destination, params string[] keys)
            {
                destination = AddRedisKeyPrefix(destination);
                RedisKey[] keyList = ConvertRedisKeysAddSysCustomKey(keys);
                var rValue = redisDB.SortedSetCombineAndStore(operation, destination, keyList);
                return rValue;
            }

            /// <summary>
            /// 获取几个集合的交叉并集合,并保存到一个新Key中
            /// </summary>
            /// <param name="operation">Union：并集  Intersect：交集  Difference：差集  详见 <see cref="SetOperation"/></param>
            /// <param name="destination">保存的新Key名称</param>
            /// <param name="keys">要操作的Key集合</param>
            /// <returns></returns>
            private  async Task<long> _SortedSetCombineAndStoreAsync(SetOperation operation, string destination, params string[] keys)
            {
                destination = AddRedisKeyPrefix(destination);
                RedisKey[] keyList = ConvertRedisKeysAddSysCustomKey(keys);
                var rValue = await redisDB.SortedSetCombineAndStoreAsync(operation, destination, keyList);
                return rValue;
            }

            #endregion
        }
    }
}
