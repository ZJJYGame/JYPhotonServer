using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;
namespace HttpServer
{
    public class CosmosHttpServer
    {
        static List<string> strList=new List<string>();
        static void Main(string[] args)
        {
            Utility.Debug.SetHelper(new ConsoleDebugHelper());
            Console.ReadKey();
        }
    }
}
