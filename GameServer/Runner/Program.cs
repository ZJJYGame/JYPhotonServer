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

            IDataContract dc;
            C2SInput input = new C2STransformInput();
            input.PlayerId = 55;
            input.SessionId= 66;
            dc = input;
            var dcBytes = Utility.MessagePack.ToByteArray(dc);
            if (dcBytes == null)
            {
                Console.WriteLine("dcBytes null");
            }
            var ddc = Utility.MessagePack.ToObject<IDataContract>(dcBytes);
            if (ddc != null)
            {
                var ipt = ddc as C2SInput;
                if(ipt==null)
                {
                    Console.WriteLine("ipt null");
                }
                else
                {
                    var iptTipt = ipt as C2STransformInput;
                    if (iptTipt == null) 
                    {
                        Console.WriteLine("iptTipt null");
                    }
                    else
                    {
                        Console.WriteLine("iptTipt exit");
                    }
                }
            }
            else
            {
                Console.WriteLine("ddc null");
            }
            Console.ReadKey();
        }
    }
}
