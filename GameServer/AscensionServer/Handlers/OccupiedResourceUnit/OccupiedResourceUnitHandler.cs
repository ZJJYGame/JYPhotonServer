using AscensionProtocol;
using Cosmos;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionServer.Threads;

namespace AscensionServer
{
    /// <summary>
    /// 占用资源单位请求；
    /// 例如怪物被占用，则其他玩家无法触发战斗；
    /// 矿石被占用，则其他玩家无法拾取
    /// </summary>
    public class OccupiedResourceUnitHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.OccupiedResourceUnit;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResponseData.Clear();
            var occupiedUnitJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.OccupiedUnit));
            AscensionServer._Log.Info("请求资源数据  :  " + occupiedUnitJson);
            var occupiedUnitObj = Utility.Json.ToObject<OccupiedUnitDTO>(occupiedUnitJson);
            var result = AscensionServer.Instance.OccupiedResUnit(occupiedUnitObj);
            if (result)
            {
                OpResponse.ReturnCode = (short)ReturnCode.Success;
                var peerSet = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();

                var threadData = Singleton<ReferencePoolManager>.Instance.Spawn<ThreadData<AscensionPeer>>();
                threadData.SetData(peerSet, (byte)EventCode.OccupiedResourceUnit, peer);
                var syncEvent = Singleton<ReferencePoolManager>.Instance.Spawn<SyncOccupiedUnitEvent>();
                syncEvent.SetData(threadData);
                syncEvent.AddFinishedHandler(() => {
                    Singleton<ReferencePoolManager>.Instance.Despawns(syncEvent, threadData);
                    ThreadEvent.RemoveSyncEvent(syncEvent);
                });
                ThreadEvent.AddSyncEvent(syncEvent);
                ThreadEvent.ExecuteEvent();
            }else
                OpResponse.ReturnCode = (short)ReturnCode.Fail;
            OpResponse.Parameters = ResponseData;
            OpResponse.OperationCode = operationRequest.OperationCode;
            peer.SendOperationResponse(OpResponse, sendParameters);
            //利用池生成线程池所需要使用的对象，并为其赋值，结束时回收
        }
    }
}
