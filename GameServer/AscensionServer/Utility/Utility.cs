﻿/*
*Author : Don
*Since :	2020-05-05
*Description : 单例基类，包含线程安全类型
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace AscensionServer
{
    public   static partial class Utility
    {
        public static Value GetValue<Key, Value>(Dictionary<Key, Value> dicr, Key key)
        {
            Value value;

            bool isSuccess = dicr.TryGetValue(key, out value);
            if (isSuccess)
            {
                return value;
            }
            else
            {
                return default(Value);
            }
        }

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T DeSerialize<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
    }
}
