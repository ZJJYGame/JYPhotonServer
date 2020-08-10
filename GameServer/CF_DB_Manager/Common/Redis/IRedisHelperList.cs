using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;
using MagicOnion;
using System.Threading.Tasks;

namespace ProtocolCore
{
    public interface IRedisHelperList : IService<IRedisHelperList>
    {
        /// <summary>
        /// 从左侧向list中添加一个值，返回集合总数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVar"></param>
        /// <returns></returns>
        Task<UnaryResult<long>> ListLeftPushAsync(string key, string[] jsonVar);
        /// <summary>
        /// 从右侧向list中添加一个值，返回集合总数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVar"></param>
        /// <returns></returns>
        Task<UnaryResult<long>> ListRightPushAsync(string key, string[] jsonVar);
        /// <summary>
        /// 从左侧向list中取出一个值并从list中删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<UnaryResult<string>> ListLeftPopAsync(string key);
        /// <summary>
        /// 从右侧向list中取出一个值并从list中删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<UnaryResult<string>> ListRightPopAsync(string key);
        /// <summary>
        /// 从key的List中右侧取出一个值，并从左侧添加到destination集合中，且返回该数据对象
        /// </summary>
        /// <param name="key">要取出数据的List名称</param>
        /// <param name="destination">要添加到的List名称</param>
        /// <returns></returns>
        Task<UnaryResult<string>> ListRightPopLeftPushAsync(string key, string destination);
        /// <summary>
        /// 在key的List指定值pivot之后插入value，返回集合总数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pivot">索引值</param>
        /// <param name="jsonVar">要插入的值</param>
        /// <returns></returns>
        Task<UnaryResult<long>> ListInsertAfterAsync(string key, string pivot, string jsonVar);
        /// <summary>
        /// 在key的List指定值pivot之前插入value，返回集合总数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pivot">索引值</param>
        /// <param name="jsonVar">要插入的值</param>
        /// <returns></returns>
        Task<UnaryResult<long>> ListInsertBeforeAsync(string key, string pivot, string jsonVar);
        /// <summary>
        /// 从key的list中取出所有数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<UnaryResult<string[]>> ListRangeAsync(string key);
        /// <summary>
        /// 从key的List获取指定索引的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<UnaryResult<string>> ListGetByIndexAsync(string key, long index);
        /// <summary>
        /// 获取key的list中数据个数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<UnaryResult<long>> ListLengthAsync(string key);
        /// <summary>
        /// 从key的List中移除指定的值，返回删除个数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonVar"></param>
        /// <returns></returns>
        Task<UnaryResult<long>> ListRemoveAsync(string key, string jsonVar);
    }
}
