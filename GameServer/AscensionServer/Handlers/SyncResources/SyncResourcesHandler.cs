/*
 * 
 * Description: 同步加载状态
 * Since : 2020 -07-07
 * Author :xianenzhang*/



using AscensionProtocol;
using Photon.SocketServer;
using System;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionServer.Threads;
using System.Collections.Generic;


namespace AscensionServer
{
    public class SyncResourcesHandler: Handler
    {
        HashSet<ResourcesDTO> resSet = new HashSet<ResourcesDTO>();
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncResources;
            base.OnInitialization();
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResponseData.Clear();
            var roleMoveStatusJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.SyncResourcesLoad));
            /*
            peer.RoleMoveStatusJson = roleMoveStatusJson;
            peer.RoleMoveStatus = Utility.Json.ToObject<RoleMoveStatusDTO>(roleMoveStatusJson);
            AscensionServer._Log.Info("Role:ID " + peer.PeerCache.RoleID + "\n RoleJson :" + roleMoveStatusJson);

            resSet.Clear();
            var peerSet = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            int peerSetLength = peerSet.Count;
            for (int i = 0; i < peerSetLength; i++)
            {
                resSet.Add(peerSet[i].RoleMoveStatus);
            }
            var roleSetJson = Utility.Json.ToJson(resSet);

            ResponseData.Add((byte)ParameterCode.RoleMoveStatus, roleSetJson);
            OpResponse.OperationCode = operationRequest.OperationCode;
            OpResponse.ReturnCode = (short)ReturnCode.Success;
            OpResponse.Parameters = ResponseData;
            peer.SendOperationResponse(OpResponse, sendParameters);


            var threadData = Singleton<ReferencePoolManager>.Instance.Spawn<ThreadData<AscensionPeer>>();
            threadData.SetData(peerSet, (byte)EventCode.SyncRoleMoveStatus, peer);
            var syncEvent = Singleton<ReferencePoolManager>.Instance.Spawn<SyncRoleMoveStatusEvent>();
            syncEvent.SetData(threadData);
            syncEvent.AddFinishedHandler(() => {
                Singleton<ReferencePoolManager>.Instance.Despawns(syncEvent, threadData);
                ThreadEvent.RemoveSyncEvent(syncEvent);
            });
            ThreadEvent.AddSyncEvent(syncEvent);
            ThreadEvent.ExecuteEvent();*/
        }
    }
}
