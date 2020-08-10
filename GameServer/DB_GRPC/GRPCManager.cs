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
    public class GRPCManager : ConcurrentSingleton<GRPCManager>, IBehaviour
    {
        string host = "localhost";
        ushort port = 12345;
        public Channel Channel { get; private set; }
        public void OnInitialization()
        {
            Channel = new Channel(host, port, ChannelCredentials.Insecure);
            Channel.ConnectAsync();
        }
        public void OnTermination()
        {
        }
    }
}
