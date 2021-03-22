using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;
namespace HttpServer
{
    public class CosmosHttpServer
    {

        static HttpClientPeer clientPeer;

        static void Main(string[] args)
        {
            Utility.Debug.SetHelper(new ConsoleDebugHelper());
            clientPeer = new HttpClientPeer("http://localhost:8080/");
            clientPeer.Run();
            while (true)
            {
                var str = Console.ReadLine();
                clientPeer.Post(str, (dat) =>
                {
                    Utility.Debug.LogInfo(dat);
                });
            }
        }
    }
}
