/*
 * Author : xianrenZhang
 * Since : 2020-07-02
 * Description ：同步移动状态
 * */
using AscensionProtocol;
using Photon.SocketServer;
using System;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionServer.Threads;

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
            ResponseData.Clear();
            var roleMoveStatusJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleMoveStatus));
            peer.RoleMoveStatusJson = roleMoveStatusJson;
            peer.RoleMoveStatus = Utility.Json.ToObject<RoleMoveStatusDTO>(roleMoveStatusJson);
            AscensionServer._Log.Info("Role:ID " + peer.PeerCache.RoleID + "\n RoleJson :" + roleMoveStatusJson);
            OpResponse.OperationCode = operationRequest.OperationCode;
            OpResponse.ReturnCode = (short)ReturnCode.Success;
            peer.SendOperationResponse(OpResponse, sendParameters);
        
            var peerSet =  AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            var threadData = Singleton<ReferencePoolManager>.Instance.Spawn<ThreadData<AscensionPeer>>();
            threadData.SetData(peerSet, (byte)EventCode.SyncRoleMoveStatus, peer);
            var syncEvent = Singleton<ReferencePoolManager>.Instance.Spawn<SyncRoleMoveStatusEvent>();
            syncEvent.SetData(threadData);
            syncEvent.AddFinishedHandler(() => {
                Singleton<ReferencePoolManager>.Instance.Despawns(syncEvent, threadData);
                ThreadEvent.RemoveSyncEvent(syncEvent);
            });
            ThreadEvent.AddSyncEvent(syncEvent);
            ThreadEvent.ExecuteEvent();
        }
    }
}
