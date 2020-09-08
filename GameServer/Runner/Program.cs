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
            //RedisHelper.String.StringSet("FQU-ALL", "FQU2-ALL-000", new TimeSpan(0, 0, 0, 60));
            //var result= RedisHelper.String.StringSetAsync("String", "String", new TimeSpan(0, 0, 0, 60));
            //RedisHelper.String.StringGetSet("FQU-ALL-2", "aijwdiajhdii");
            //RedisHelper.String.StringAppend("FQU-ALL-2", "------awmdiamdaijwdiajhdii");
            //RedisHelper.Set.SetAdd("FQU-ALL-4", "12");
            //RedisHelper.Set.SetAdd("FQU-ALL-4", "13");
            //List<string> lset = new List<string>() { "a", "d", "84" };
            //RedisHelper.List.ListLeftPush<string>("Aaa", lset);
            //Console.WriteLine("Hello World!");
            //Guid guid = Guid.NewGuid();
            //Console.ReadKey();
            //var b = Utility.Algorithm.CreateRandomInt(8, 10000, 99999999);
            //Console.WriteLine($"Convert.ToInt32 : {b}");


            Dictionary<int, RefClass> dict = new Dictionary<int, RefClass>();
            RefClass rc1 = new RefClass(1,2);
            RefClass rc2 = new RefClass(3,4);
            dict.Add(1, rc1);
            dict.Add(2, rc2);
            Console.WriteLine($"Dict: {Utility.Json.ToJson(dict)}");
            rc1.a = 5;
            rc1.b= 6;
            Console.WriteLine($"RC1: {Utility.Json.ToJson(rc1)}");
            Console.WriteLine($"Dict: {Utility.Json.ToJson(dict)}");
            Console.ReadLine();
        }
        [Serializable]
        class RefClass
        {
            public int a;
            public int b;
            public RefClass(int a, int b)
            {
                this.a = a;
                this.b = b;
            }
        }

    }
}
