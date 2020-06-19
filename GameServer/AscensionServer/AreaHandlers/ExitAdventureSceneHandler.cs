using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using Cosmos;
using AscensionProtocol;
using AscensionServer.Threads;

namespace AscensionServer
{
    public class ExitAdventureSceneHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.ExitAdventureScene;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            //var resultJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            AscensionServer._Log.Info( "ExitAdventureScene  : "+ peer.ToString());
            AscensionServer.Instance.ExitAdventureScene(peer);
            //这条，获取玩家已经离开探索界面时候所有玩家的集合
            var peerSet = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();

            ResponseData.Clear();
            OpResponse.OperationCode = operationRequest.OperationCode;
            OpResponse.ReturnCode = (byte)ReturnCode.Success;
            peer.SendOperationResponse(OpResponse, sendParameters);

            //利用池生成线程池所需要使用的对象，并为其赋值，结束时回收
            var threadData = Singleton<ReferencePoolManager>.Instance.Spawn<ThreadData<AscensionPeer>>();
            threadData.SetData(peerSet, (byte)EventCode.DeletePlayer, peer);
            var syncEvent = Singleton<ReferencePoolManager>.Instance.Spawn<SyncOtherRoleEvent>();
            syncEvent.SetData(threadData);
            syncEvent.AddFinishedHandler(() => { Singleton<ReferencePoolManager>.Instance.Despawns(syncEvent, threadData); });
            ThreadEvent.AddSyncEvent(syncEvent);
            ThreadEvent.ExecuteEvent();
        }
    }
}
