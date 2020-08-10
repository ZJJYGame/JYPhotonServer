using MagicOnion;
using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;
using System.Threading.Tasks;
using MessagePack;

namespace ProtocolCore
{
    public interface IRedisHelper : IService<IRedisHelper>
    {
        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key">要判断的key</param>
        /// <returns></returns>
        Task<UnaryResult<bool>> KeyExistsAsync(string key);
        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">要删除的key</param>
        /// <returns>是否删除成功</returns>
        Task<UnaryResult<bool>> KeyDeleteAsync(string key);
        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">要删除的key集合</param>
        /// <returns>成功删除的个数</returns>
        Task<UnaryResult<long>> KeysDeleteAsync(params string[] keys);
        /// <summary>
        /// 清空当前DataBase中所有Key
        /// </summary>
        Task<UnaryResult<object>> KeyFulshAsync();
        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        Task<UnaryResult<bool>> KeyRenameAsync(string key, string newKey);
        /// <summary>
        /// 设置Key的过期时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        Task<UnaryResult<bool>> KeyExpireAsync(string key, TimeSpan? expiry = default(TimeSpan?));
    }
}
