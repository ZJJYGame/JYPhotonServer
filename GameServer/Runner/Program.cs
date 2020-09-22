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

            var result =Utility.Assembly.GetInstancesByAttribute<NetworkHandlerAttribute, IPersion>();
            for (int i = 0; i < result.Length; i++)
            {
                Console.WriteLine(result[i].GetType());
            }

            Console.ReadKey();
        }
    }
}
