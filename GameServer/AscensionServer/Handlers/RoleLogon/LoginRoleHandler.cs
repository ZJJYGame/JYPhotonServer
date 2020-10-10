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
            IRemotePeer peer = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.ClientPeer) as IRemotePeer;
            var json = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role));
            var roleObj = Utility.Json.ToObject<RoleDTO>(json);
            var role = RemoteRole.Create(roleObj.RoleID, peer.SessionId, roleObj);

            IRemoteRole remoteRole;
            var roleExist = GameManager.CustomeModule<RoleManager>().TryGetValue(roleObj.RoleID, out remoteRole);
            if (roleExist)
            {
                IPeerAgent pa;
                GameManager.CustomeModule<PeerManager>().TryGetValue(remoteRole.SessionId, out pa);
                pa.SendEventMessage((byte)EventCode.ReplacePlayer, null);//从这里发送挤下线消息；
                GameManager.CustomeModule<RoleManager>().TryRemove(roleObj.RoleID);
            }
            IPeerAgent peerAgent;
            var result = GameManager.CustomeModule<PeerManager>().TryGetValue(peer.SessionId, out peerAgent);
            if (result)
            {
                Utility.Debug.LogInfo("yzqData" + "验证登录的Sessionid:" + peer.SessionId);
                var remoteRoleType = typeof(IRemoteRole);
                var exist = peerAgent.ContainsKey(remoteRoleType);
                if (!exist)
                {
                    GameManager.CustomeModule<RoleManager>().TryAdd(roleObj.RoleID, role);
                    Utility.Debug.LogInfo("yzqData" + "进入角色判断只有一个账号选择角色");
                    peerAgent.TryAdd(remoteRoleType, role);
                    operationResponse.ReturnCode = (byte)ReturnCode.Success;
                }
                else
                {
                    Utility.Debug.LogInfo("yzqData" + "已有账号登陆角色");
                    object legacyRole;
                    peerAgent.TryGetValue(remoteRoleType, out legacyRole);
                    var remoteRoleObj = legacyRole as IRemoteRole;
                    var updateResult = peerAgent.TryUpdate(remoteRoleType, role, legacyRole);
                    if (updateResult)
                    {
                        Utility.Debug.LogInfo("yzqData" + "已有账号登陆角色，替换相同角色成功");

                        GameManager.CustomeModule<RoleManager>().TryRemove(roleObj.RoleID);
                        GameManager.CustomeModule<RoleManager>().TryAdd(roleObj.RoleID, role);
                        operationResponse.ReturnCode = (byte)ReturnCode.Success;//登录成功
                        GameManager.CustomeModule<RecordManager>().RecordRole(remoteRoleObj.RoleId, role);
                        GameManager.ReferencePoolManager.Despawn(remoteRoleObj);//回收这个RemoteRole对象
                    }
                    else
                    {
                        Utility.Debug.LogInfo("yzqData" + "替换角色失败");
                        operationResponse.ReturnCode = (byte)ReturnCode.Fail;//登录失败
                    }

                }
            }
            return operationResponse;
        }
    }
}
