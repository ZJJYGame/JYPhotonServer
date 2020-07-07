using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using Cosmos;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Threads;
using AscensionServer.Model;

namespace AscensionServer
{
    public class EnterAdventureSceneHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.EnterAdventureScene;
            base.OnInitialization();
        }
        HashSet<Role> roleSet = new HashSet<Role>();
        HashSet<RoleMoveStatusDTO> roleMoveStatusSet = new HashSet<RoleMoveStatusDTO>();
        HashSet<RoleTransformQueueDTO> roleTransformQueueSet = new HashSet<RoleTransformQueueDTO>();
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var resultJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role));
            var moveStatusJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleMoveStatus));
            AscensionServer._Log.Info("EnterAdventureScene  :  " + peer.ToString());
            //这条，获取当前玩家未进入探索界面时候所有玩家的集合
            var peerSet = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            AscensionServer.Instance.EnterAdventureScene(peer);

            peer.RoleMoveStatus = Utility.Json.ToObject<RoleMoveStatusDTO>(moveStatusJson);
            int peerSetLength = peerSet.Count;
             roleSet.Clear();
            roleMoveStatusSet.Clear();
            roleTransformQueueSet.Clear();
            for (int i = 0; i < peerSetLength; i++)
            {
                roleSet.Add( peerSet[i].PeerCache.Role);
                roleMoveStatusSet.Add(peerSet[i].RoleMoveStatus);
                roleTransformQueueSet.Add(peerSet[i].RoleTransformQueueDTO);
            }
            var roleSetJson = Utility.Json.ToJson(roleSet);
            var roleMoveStatusSetJson = Utility.Json.ToJson(roleMoveStatusSet);
            var roleTransformQueueSetJson = Utility.Json.ToJson(roleTransformQueueSet);
            ResponseData.Clear();
            OpResponse.OperationCode = operationRequest.OperationCode;
            OpResponse.ReturnCode = (byte)ReturnCode.Success;
            ResponseData.Add((byte)ParameterCode.RoleSet, roleSetJson);
            ResponseData.Add((byte)ParameterCode.RoleMoveStatusSet, roleMoveStatusSetJson);
            ResponseData.Add((byte)ParameterCode.RoleTransformQueueSet,roleTransformQueueSetJson);
            OpResponse.Parameters = ResponseData;
            peer.SendOperationResponse(OpResponse, sendParameters);

            //利用池生成线程池所需要使用的对象，并为其赋值，结束时回收
            var threadData = Singleton<ReferencePoolManager>.Instance.Spawn<ThreadData<AscensionPeer>>();
            threadData.SetData(peerSet, (byte)EventCode.NewPlayer, peer);
            var syncEvent = Singleton<ReferencePoolManager>.Instance.Spawn<SyncOtherRoleEvent>();
            syncEvent.SetData(threadData);
            syncEvent.AddFinishedHandler(() => { Singleton<ReferencePoolManager>.Instance.Despawns(syncEvent,threadData);
                ThreadEvent.RemoveSyncEvent(syncEvent); });
            ThreadEvent.AddSyncEvent(syncEvent);
            ThreadEvent.ExecuteEvent();

        }
    }
}
