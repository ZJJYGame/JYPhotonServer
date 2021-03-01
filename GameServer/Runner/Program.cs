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
            //Utility.Json.SetHelper(new NewtonjsonHelper());
            //Utility.MessagePack.SetHelper(new ImplMessagePackHelper());
            //Random random = new Random();

            CosmosEntry.LaunchHelpers();


            //CosmosEntry.LaunchModules();
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //for (int i = 0; i < assemblies.Length; i++)
            //{
            //    var types = assemblies[i].GetTypes();
            //    for (int j = 0; j < types.Length; j++)
            //    {
            //        if (typeof(Cosmos.Module).IsAssignableFrom(types[j]))
            //        {
            //            Utility.Debug.LogInfo(types[j].Name);
            //        }
            //    }
            //}


            //var assembly = typeof(ParMgr).GetType().Assembly;
            //var types = assembly.GetTypes();
            //for (int i = 0; i < types.Length; i++)
            //{
            //    if (typeof(Cosmos.Module).IsAssignableFrom(types[i]))
            //    {
            //        Utility.Debug.LogInfo(types[i].Name);
            //    }
            //}
            var result = 1 << 3;
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}
