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
            //Console.WriteLine(path);
            //Utility.IO.WriteTextFile(path, "iobin.json", "这里是IO测试文件111");
            //if (true)
            //{
            //    var types = Utility.Assembly.GetDerivedTypes(typeof(IPersion));
            //    for (int i = 0; i < types.Length; i++)
            //    {
            //        if (types[i].GetCustomAttributes(typeof(InheritedAttribute), true).Length > 0)
            //            Console.WriteLine(types[i] + " true\n");
            //        else
            //            Console.WriteLine(types[i] + " false\n");
            //    }
            //}
            Console.ReadKey();
        }
    }
}
