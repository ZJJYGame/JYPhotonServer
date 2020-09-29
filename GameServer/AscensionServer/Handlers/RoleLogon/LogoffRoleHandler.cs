using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Cosmos;
using Photon.SocketServer;

namespace AscensionServer
{
    /// <summary>
    /// 角色登出；
    /// 这里使用了类似ECS的结构，RemoteRole对象可以作为Peer的数据负载存在；
    /// </summary>
    public class LogoffRoleHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.LogoffRole; } }
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            IRemotePeer peer = Utility.GetValue(operationResponse.Parameters, (byte)ParameterCode.ClientPeer) as IRemotePeer;
            var json = Convert.ToString(Utility.GetValue(operationResponse.Parameters, (byte)ParameterCode.Role));
            var roleObj = Utility.Json.ToObject<RoleDTO>(json);
            IPeerAgent peerAgent;
            var result = GameManager.CustomeModule<PeerManager>().TryGetValue(peer.SessionId, out peerAgent);
            if (result)
            {
                var remoteRoleType = typeof(IRemoteRole);
                object remoteRoleObj;
                var removeResult= peerAgent.TryRemove(remoteRoleType, out remoteRoleObj);
                if (removeResult)
                {
                    var remoteRole = remoteRoleObj as IRemoteRole;
                    GameManager.ReferencePoolManager.Despawn(remoteRole);//回收这个RemoteRole对象
                }
                operationResponse.ReturnCode = (byte)ReturnCode.Success;
            }
            else
            {
                operationResponse.ReturnCode = (byte)ReturnCode.ItemNotFound;
            }
            return operationResponse;
        }
    }
}
