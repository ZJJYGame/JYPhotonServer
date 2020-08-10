using System;
using StackExchange.Redis;
using Cosmos;
using RedisDotNet;
using System.Collections.Generic;
namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            Utility.Json.SetHelper(new NewtonjsonHelper());
            RedisManager.Instance.OnInitialization();
            RedisHelper.String.StringSet("FQU-ALL", "FQU2-ALL-000", new TimeSpan(0, 0, 0, 60));
            RedisHelper.String.StringGetSet("FQU-ALL-2", "aijwdiajhdii");
            RedisHelper.String.StringAppend("FQU-ALL-2", "------awmdiamdaijwdiajhdii");
            RedisHelper.Set.SetAdd("FQU-ALL-4", "12");
            RedisHelper.Set.SetAdd("FQU-ALL-4", "13");
            List<string> lset = new List<string>() { "a", "d", "84" };
            RedisHelper.List.ListLeftPush<string>("Aaa", lset);
            Console.WriteLine("Hello World!");
            Console.ReadLine();

        }
    }
}
