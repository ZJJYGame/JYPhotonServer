using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisDotNet;

namespace AscensionServer
{
    public partial class RedisData
    {
        /// <summary>
        /// 数据库数据的向Redis储存的处理类
        /// </summary>
        public  class ReidsDataProcessing
        {
            public static string GetData(string key)
            {
                if (RedisHelper.KeyExistsAsync(key).Result)
                {
                   return RedisHelper.String.StringGetAsync(key).Result;
                }
                else
                {                   
                    return null;
                }
            }
            public static string InsertName(string header,int id)
            {
                var name = header + id;
                return name;
            }
            public static string InsertName(string header)
            {
                var name = header ;
                return name;
            }

            /// <summary>
            /// 获取Redis中储存的String
            /// </summary>
            /// <param name="key">对应的RedisKeyDefine中的值</param>
            /// <param name="RoleID"></param>
            /// <returns></returns>
            public static string GetRedisData(string key, int RoleID)
            {
                var result = GetData(key + RoleID);
                if (!string.IsNullOrEmpty(result))
                {
                    var dict = RedisHelper.String.StringGet(key + RoleID);
                    return dict;
                }
                return null;
            }
            /// <summary>
            /// 获取redis中储存的Hash
            /// </summary>
            /// <typeparam name="T">获取的对象类型</typeparam>
            /// <param name="key">对应的RedisKeyDefine中的值</param>
            /// <param name="RoleID"></param>
            /// <returns></returns>
            public static object GetRedisData<T>(string key, int RoleID)
            {
                var result = GetData(key + RoleID);
                if (!string.IsNullOrEmpty(result))
                {
                    var dict = RedisHelper.Hash.HashGet<T>(key+RoleID, RoleID.ToString());
                    return dict;
                }
                return null;
            }
        }
    }
}
