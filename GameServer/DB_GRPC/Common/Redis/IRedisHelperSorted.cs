using System.Collections.Generic;
using System.Threading.Tasks;
using MagicOnion;

namespace ProtocolCore
{
    public interface IRedisHelperSorted : IService<IRedisHelperSorted>
    {
        /// <summary>
        /// 添加一个值到Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVar"></param>
        /// <param name="score">排序分数，为空将获取集合中最大score加1</param>
        /// <returns></returns>
       Task< UnaryResult<bool>> SortedSetAddAsync(string key, string jsonVar, double? score = null);
        Task< UnaryResult<long>> SortedSetAddAsync(string key, string[] jsonVars, double? score = null);

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<UnaryResult<long>> SortedSetLengthAsync(string key);
        /// <summary>
        /// 获取指定起始值到结束值的集合数量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="startValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <returns></returns>
        Task<UnaryResult<long>> SortedSetLengthByValueAsync(string key, string startValue, string endValue);
        /// <summary>
        /// 获取指定Key的排序Score值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVar"></param>
        /// <returns></returns>
        Task<UnaryResult<double?>> SortedSetScoreAsync(string key, string jsonVar);
        /// <summary>
        /// 获取指定Key中最小Score值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<UnaryResult<double>> SortedSetMinScoreAsync(string key);
        /// <summary>
        /// 获取指定Key中最大Score值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<UnaryResult<double>> SortedSetMaxScoreAsync(string key);
        /// <summary>
        /// 删除Key中指定的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        Task<UnaryResult<long>> SortedSetRemoveAsync(string key, params string[] value);
        /// <summary>
        /// 删除指定起始值到结束值的数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="startValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <returns></returns>
        Task<UnaryResult<long>> SortedSetRemoveRangeByValueAsync(string key, string startValue, string endValue);
        /// <summary>
        /// 删除 从 start 开始的 stop 条数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        Task<UnaryResult<long>> SortedSetRemoveRangeByRankAsync(string key, long start, long stop);
        /// <summary>
        /// 根据排序分数Score，删除从 start 开始的 stop 条数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        Task<UnaryResult<long>> SortedSetRemoveRangeByScoreAsync(string key, double start, double stop);
        /// <summary>
        /// 获取从 start 开始的 stop 条数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">起始数</param>
        /// <param name="stop">-1表示到结束，0为1条</param>
        /// <param name="desc">是否按降序排列</param>
        /// <returns></returns>
        Task<UnaryResult<string[]>> SortedSetRangeByRankAsync(string key, long start = 0, long stop = -1, bool desc = false);
        /// <summary>
        /// 获取从 start 开始的 stop 条数据包含Score，返回数据格式：Key=值，Value = Score
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">起始数</param>
        /// <param name="stop">-1表示到结束，0为1条</param>
        /// <param name="desc">是否按降序排列</param>
        /// <returns></returns>
        Task<UnaryResult<Dictionary<string, double>>> SortedSetRangeByRankWithScoresAsync(string key, long start = 0, long stop = -1, bool desc = false);
        /// <summary>
        ///  根据Score排序 获取从 start 开始的 stop 条数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">起始数</param>
        /// <param name="stop">-1表示到结束，0为1条</param>
        /// <param name="desc">是否按降序排列</param>
        /// <returns></returns>
        Task<UnaryResult<string[]> >SortedSetRangeByScoreAsync(string key, double start = 0, double stop = -1, bool desc = false);
        /// <summary>
        /// 根据Score排序  获取从 start 开始的 stop 条数据包含Score，返回数据格式：Key=值，Value = Score
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">起始数</param>
        /// <param name="stop">-1表示到结束，0为1条</param>
        /// <param name="desc">是否按降序排列</param>
        /// <returns></returns>
        Task<UnaryResult<Dictionary<string, double>>> SortedSetRangeByScoreWithScoresAsync(string key, double start = 0, double stop = -1, bool desc = false);
        /// <summary>
        /// 获取指定起始值到结束值的数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="startValue">起始值</param>
        /// <param name="endValue">结束值</param>
        /// <returns></returns>
        Task<UnaryResult<string[]>> SortedSetRangeByValueAsync(string key, string startValue, string endValue);
        /// <summary>
        /// 获取几个集合的并集,并保存到一个新Key中
        /// </summary>
        /// <param name="destination">保存的新Key名称</param>
        /// <param name="keys">要操作的Key集合</param>
        /// <returns></returns>
        Task<UnaryResult<long>> SortedSetCombineUnionAndStoreAsync(string destination, params string[] keys);
        /// <summary>
        /// 获取几个集合的交集,并保存到一个新Key中
        /// </summary>
        /// <param name="destination">保存的新Key名称</param>
        /// <param name="keys">要操作的Key集合</param>
        /// <returns></returns>
        Task<UnaryResult<long>> SortedSetCombineIntersectAndStoreAsync(string destination, params string[] keys);
        /// <summary>
        /// 修改指定Key和值的Scores在原值上减去scores，并返回最终Scores
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVar"></param>
        /// <param name="scores"></param>
        /// <returns></returns>
        Task<UnaryResult<double>> SortedSetDecrementAsync(string key, string jsonVar, double scores);
        /// <summary>
        /// 修改指定Key和值的Scores在原值上增加scores，并返回最终Scores
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVar"></param>
        /// <param name="scores"></param>
        /// <returns></returns>
        Task<UnaryResult<double>> SortedSetIncrementAsync(string key, string jsonVar, double scores);
    }
}
