﻿using System;
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
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            //var resultJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            AscensionServer._Log.Info("EnterAdventureScene  :  " + peer.ToString());
            //这条，获取当前玩家未进入探索界面时候所有玩家的集合
            var peerSet = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            int peerSetLength = peerSet.Count;
             roleSet.Clear();
            for (int i = 0; i < peerSetLength; i++)
            {
                roleSet.Add( peerSet[i].PeerCache.Role);
            }
            var roleSetJson = Utility.Json.ToJson(roleSet);

            ResponseData.Clear();
            OpResponse.OperationCode = operationRequest.OperationCode;
            OpResponse.ReturnCode = (byte)ReturnCode.Success;
            ResponseData.Add((byte)ParameterCode.RoleSet, roleSetJson);
            OpResponse.Parameters = ResponseData;
            peer.SendOperationResponse(OpResponse, sendParameters);

            AscensionServer.Instance.EnterAdventureScene(peer);

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