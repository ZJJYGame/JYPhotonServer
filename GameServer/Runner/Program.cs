using System;
using StackExchange.Redis;
using Cosmos;
using RedisDotNet;
using System.Collections.Generic;
using System.Reflection;
using Protocol;
namespace Runner
{
    class Program
    {
        static string path = Environment.CurrentDirectory + "/IO";
        static void Main(string[] args)
        {
            Utility.Json.SetHelper(new NewtonjsonHelper());
            Utility.MessagePack.SetHelper(new ImplMessagePackHelper());
            Random random = new Random();
            Console.ReadKey();
        }
    }
}
