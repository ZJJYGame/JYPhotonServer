using AscensionProtocol;
using Photon.SocketServer;
using System;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionServer.Threads;
using System.Collections.Generic;
namespace AscensionServer
{
    public class SyncResourcesEventHandler: Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncResourcesEvent;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResponseData.Clear();
            var resIdJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.SyncResourcesLoad));
            var resObj = Utility.Json.ToObject<Vector2DTO>(resIdJson);
            peer.PeerCache.Vect2 = resObj;
            AscensionServer._Log.Info("ID " + peer.PeerCache.RoleID + "\n RoleJson :" + resObj);
            foreach (var item in AscensionServer.Instance.Resources)
            {
               
                if (item.Vector3.posX == resObj.posX && item.Vector3.posZ == resObj.posY)
                {
                    //AscensionServer.Instance.Resources.Remove(item);
                    OpResponse.OperationCode = operationRequest.OperationCode;
                    OpResponse.ReturnCode = (short)ReturnCode.Success;
                    OpResponse.Parameters = ResponseData;
                }
                else
                {
                    OpResponse.ReturnCode = (short)ReturnCode.Fail;
                }
            }
           
            peer.SendOperationResponse(OpResponse, sendParameters);

            var peerSet = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            var threadData = Singleton<ReferencePoolManager>.Instance.Spawn<ThreadData<AscensionPeer>>();
            threadData.SetData(peerSet, (byte)EventCode.SyncItemResources, peer);
            var syncEvent = Singleton<ReferencePoolManager>.Instance.Spawn<SyncResourcesEvent>();
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
