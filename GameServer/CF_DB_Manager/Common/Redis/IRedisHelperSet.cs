using System.Collections.Generic;
using System.Threading.Tasks;
using MagicOnion;

namespace ProtocolCore
{
    public interface IRedisHelperSet : IService<IRedisHelperSet>
    {
        /// <summary>
        /// 在Key集合中添加一个value值
        /// </summary>
        /// <param name="key">Key名称</param>
        /// <param name="value">值</param>
        /// <returns></returns>
       Task< UnaryResult<bool>> SetAddAsync(string key, string jsonVar);
        /// <summary>
        /// 获取key集合值的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<UnaryResult<long>> SetLengthAsync(string key);
        /// <summary>
        /// 判断Key集合中是否包含指定的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">要判断是值</param>
        /// <returns></returns>
        Task<UnaryResult<bool>> SetContainsAsync(string key, string jsonVar);
        /// <summary>
        /// 随机获取key集合中的一个值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<UnaryResult<string>> SetRandomMemberAsync(string key);
        /// <summary>
        /// 获取key所有值的集合
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<UnaryResult<string[]>> SetMembersAsync(string key);
        /// <summary>
        /// 删除key集合中指定的value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<UnaryResult<long>> SetRemoveAsync(string key, params string[] jsonVar);
        /// <summary>
        /// 随机删除key集合中的一个值，并返回该值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<UnaryResult<string>> SetPopAsync(string key);
        /// <summary>
        /// 获取几个集合的并集
        /// </summary>
        /// <param name="keys">要操作的Key集合</param>
        /// <returns></returns>
        Task<UnaryResult<string[]>> SetCombineUnionAsync(params string[] keys);
        /// <summary>
        /// 获取几个集合的交集
        /// </summary>
        /// <param name="keys">要操作的Key集合</param>
        /// <returns></returns>
        Task<UnaryResult<string[]>> SetCombineIntersectAsync(params string[] keys);
        /// <summary>
        /// 获取几个集合的差集
        /// </summary>
        /// <param name="keys">要操作的Key集合</param>
        /// <returns></returns>
        Task<UnaryResult<string[]>> SetCombineDifferenceAsync(params string[] keys);
        /// <summary>
        /// 获取几个集合的并集,并保存到一个新Key中
        /// </summary>
        /// <param name="destination">保存的新Key名称</param>
        /// <param name="keys">要操作的Key集合</param>
        /// <returns></returns>
        Task<UnaryResult<long>> SetCombineUnionAndStoreAsync(string destination, params string[] keys);
        /// <summary>
        /// 获取几个集合的交集,并保存到一个新Key中
        /// </summary>
        /// <param name="destination">保存的新Key名称</param>
        /// <param name="keys">要操作的Key集合</param>
        /// <returns></returns>
        Task<UnaryResult<long>> SetCombineIntersectAndStoreAsync(string destination, params string[] keys);
        /// <summary>
        /// 获取几个集合的差集,并保存到一个新Key中
        /// </summary>
        /// <param name="destination">保存的新Key名称</param>
        /// <param name="keys">要操作的Key集合</param>
        /// <returns></returns>
        Task<UnaryResult<long>> SetCombineDifferenceAndStoreAsync(string destination, params string[] keys);
    }
}
