using System;
using StackExchange.Redis;
using Cosmos;
using RedisDotNet;
using System.Collections.Generic;
using System.Reflection;

namespace Runner
{
    class Program
    {
        static string path = Environment.CurrentDirectory + "/IO";
        static void Main(string[] args)
        {
            RedisManager.Instance.OnInitialization();
            //RedisManager.Instance.SubscribeKeyExpire((key) => { Console.WriteLine($"EXPIRED: {key}"); });
            RedisHelper.String.StringSet("ZHENG","123456",new TimeSpan(0,0,5));
            Console.WriteLine("Listening for events...");
            Console.ReadKey();
        }
    }
}
