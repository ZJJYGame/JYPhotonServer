using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicOnion;

namespace ProtocolCore
{
    public interface IRedisHelperString : IService<IRedisHelperString>
    {
        /// <summary>
        /// 异步方法 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="jsonVar">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
       Task< UnaryResult<bool>> StringSetAsync(string key, string jsonVar, TimeSpan? expiry = default(TimeSpan?));
        /// <summary>
        /// 异步方法 添加多个key/value
        /// </summary>
        /// <param name="valueList">key/value集合</param>
        /// <returns></returns>
        Task<UnaryResult<bool>> StringSetsAsync(Dictionary<string, string> valueList);
        /// <summary>
        /// 异步方法 在原有key的value值之后追加value
        /// </summary>
        /// <param name="key">追加的Key名称</param>
        /// <param name="value">追加的值</param>
        /// <returns></returns>
        Task<UnaryResult<long>> StringAppendAsync(string key, string value);
        /// <summary>
        /// 异步方法 获取单个key的值
        /// </summary>
        /// <param name="key">要读取的Key名称</param>
        /// <returns></returns>
        Task<UnaryResult<string>> StringGetAsync(string key);
        /// <summary>
        /// 异步方法 获取多个key的value值
        /// </summary>
        /// <param name="keys">要获取值的Key集合</param>
        /// <returns></returns>
        Task<UnaryResult<string[]>> StringGetsAsync(params string[] keys);
        /// <summary>
        /// 异步方法 获取旧值赋上新值
        /// </summary>
        /// <param name="key">Key名称</param>
        /// <param name="jsonVar">新值</param>
        /// <returns></returns>
        Task<UnaryResult<string>> StringGetSetAsync(string key, string jsonVar);
        /// <summary>
        /// 异步方法 获取值的长度
        /// </summary>
        /// <param name="key">Key名称</param>
        /// <returns></returns>
        Task<UnaryResult<long>> StringGetLengthAsync(string key);
        /// <summary>
        /// 异步方法 数字增长val，返回自增后的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        Task<UnaryResult<double>> StringIncrementAsync(string key, double val = 1);
        /// <summary>
        /// 异步方法 数字减少val，返回自减少的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        Task<UnaryResult<double>> StringDecrementAsync(string key, double val = 1);
    }
}
