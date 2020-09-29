using AscensionProtocol;
using Cosmos;
using MySqlX.XDevAPI.Common;
using NHibernate.Hql.Ast.ANTLR.Tree;
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
            var peer = sender as IRemotePeer;
            IPeerAgent agentPeer;
            var result = GameManager.CustomeModule<PeerManager>().TryGetValue(peer.SessionId, out agentPeer);
            if (result)
            {
                var parameters= data as Dictionary<byte,object>;
                //RemoteRole role = RemoteRole.Create();
                //agentPeer.RemoteRole;
            }
        }
        public void LogoffHandler(object sender, object data)
        {
            var peer = sender as IRemotePeer;
            IPeerAgent agent;
            var result = GameManager.CustomeModule<PeerManager>().TryGetValue(peer.SessionId, out agent);
            if (result)
            {

            }
        }
    }
}
