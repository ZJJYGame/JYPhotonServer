using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;
using System.Collections.Concurrent;
using StackExchange.Redis;

namespace RedisDotNet
{
    public class RedisManager : ConcurrentSingleton<RedisManager>
    {
        /// <summary>
        /// 连接配置
        ///// </summary>
        //readonly string ConnectStr = "192.168.0.117:6379,password=123456,DefaultDatabase=0";
        //readonly string ConnectStr = "121.37.185.220:6379,password=123456,DefaultDatabase=0";
        readonly string ConnectStr = "127.0.0.1:6379,password=jygame_%Redis,DefaultDatabase=0";
        /// <summary>
        /// Redis保存数据时候key的前缀
        /// </summary>
        internal static readonly string RedisKeyPrefix = "JY";
        /// <summary>
        /// Redis对象
        /// </summary>
        public ConnectionMultiplexer Redis { get; private set; }
        /// <summary>
        /// Redis中的DB
        /// </summary>
        public IDatabase RedisDB { get; private set; }
        public void OnInitialization()
        {
            try
            {
                Redis = ConnectionMultiplexer.Connect(ConnectStr);
                if(Redis==null)
                    throw new ArgumentNullException("Redis Connect Fail");
                RedisDB = Redis.GetDatabase();
            }
            catch (Exception)
            {
                throw new RedisConnectionException(ConnectionFailureType.UnableToConnect, "Redis Connect Fail");
            }
        }
        public void OnTermination()
        {
        }
    }
}
