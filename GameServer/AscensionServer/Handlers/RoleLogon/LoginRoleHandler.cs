using AscensionProtocol;
using AscensionProtocol.DTO;
using Cosmos;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 角色登录；
    /// 这里使用了类似ECS的结构，RemoteRole对象可以作为Peer的数据负载存在；
    /// </summary>
    public class LoginRoleHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.LoginRole; } }
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            IRemotePeer peer = Utility.GetValue(operationResponse.Parameters, (byte)ParameterCode.ClientPeer) as IRemotePeer;
            var json = Convert.ToString(Utility.GetValue(operationResponse.Parameters, (byte)ParameterCode.Role));
            var roleObj = Utility.Json.ToObject<RoleDTO>(json);
            var role = RemoteRole.Create(roleObj.RoleID, roleObj);
            IPeerAgent peerAgent;
            var result = GameManager.CustomeModule<PeerManager>().TryGetValue(peer.SessionId, out peerAgent);
            if (result)
            {
                var remoteRoleType = typeof(IRemoteRole);
                var exist = peerAgent.ContainsKey(remoteRoleType);
                if (!exist)
                {
                    peerAgent.TryAdd(remoteRoleType, role);
                    operationResponse.ReturnCode = (byte)ReturnCode.Success;
                }
                else
                {
                    object legacyRole;
                    peerAgent.TryGetValue(remoteRoleType, out legacyRole);
                    var updateResult = peerAgent.TryUpdate(remoteRoleType, role, legacyRole);
                    if (updateResult)
                        operationResponse.ReturnCode = (byte)ReturnCode.Success;//登录成功
                    else
                        operationResponse.ReturnCode = (byte)ReturnCode.Fail;//登录失败
                }
            }
            return operationResponse;
        }
    }
}
