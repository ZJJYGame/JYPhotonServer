using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using Cosmos;
namespace ProtocolCore
{
    public class GRPCManager : ConcurrentSingleton<GRPCManager>
    {
        //string host = "localhost";
        string host = "192.168.0.117";
        ushort port = 23401;
        public Channel Channel { get; private set; }
        public void OnInitialization()
        {
            Channel = new Channel(host, port, ChannelCredentials.Insecure);
            Channel.ConnectAsync();
        }
        public void InitGRPC(string host,ushort port)
        {
            this.host = host;
            this.port = port;
            OnInitialization();
        }
        public void OnTermination()
        {
        }
    }
}
