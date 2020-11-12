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
            Dictionary<Type, IDataContract> dict = new Dictionary<Type, IDataContract>();
            C2STransformInput c2sTransIpt = new C2STransformInput();
            Console.WriteLine(c2sTransIpt.GetType());
            Console.WriteLine(typeof(C2STransformInput));

            Console.ReadKey();
        }
    }
}
