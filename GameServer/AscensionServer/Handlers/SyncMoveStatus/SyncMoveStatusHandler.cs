/*
 * Author : xianrenZhang
 * Since : 2020-07-02
 * Description ：同步移动状态
 * */
using AscensionProtocol;
using Photon.SocketServer;
using System;
using Cosmos;
namespace AscensionServer
{

    public class SyncMoveStatusHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncMoveStatus;
            base.OnInitialization();
        }
        //获取客户端玩家移动状态请求的处理的代码
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var roleMoveStatusJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleMoveStatus));
            peer.RoleMoveStatusJson = roleMoveStatusJson;
            AscensionServer._Log.Info("Role:ID " + peer.PeerCache.RoleID + "\n RoleJson :" + roleMoveStatusJson);
        }
    }
}
