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
        public static class Initialize
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
        }
    }
}
