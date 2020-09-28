using AscensionProtocol;
using Cosmos;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [TargetHelper]
    public class RoleOperationHelper : IRoleOperationHelper
    {
        public void LoginHandler(object sender, object data)
        {
            var peer = sender as INetworkPeer;
            IPeerAgent agent;
            var result = GameManager.CustomeModule<PeerManager>().TryGetValue(peer.SessionId, out agent);
            if (result)
            {

            }
            //agent.SendEventMessage((byte)EventCode.NewPlayer,)
        }
        public void LogoffHandler(object sender, object data)
        {
            var peer = sender as INetworkPeer;
            IPeerAgent agent;
            var result = GameManager.CustomeModule<PeerManager>().TryGetValue(peer.SessionId, out agent);
            if (result)
            {

            }
        }
    }
}
