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
            Utility.Debug.LogInfo("请求资源数据  :  " + occupiedUnitJson);
            var occupiedUnitObj = Utility.Json.ToObject<OccupiedUnitDTO>(occupiedUnitJson);
            var result = AscensionServer.Instance.OccupiedResUnit(occupiedUnitObj);
            if (result)
            {
                OpResponse.ReturnCode = (short)ReturnCode.Success;

                ResourceUnitSetDTO currentDictObj = null;
                if (AscensionServer.Instance.ResUnitSetDict.TryGetValue(occupiedUnitObj.GlobalID,out currentDictObj))
                {

                    ResourceUnitDTO resourceUnitDTO = null;
                    if (currentDictObj.ResUnitDict.TryGetValue(occupiedUnitObj.ResID, out resourceUnitDTO))
                        resourceUnitDTO.Occupied = result;
                }
                var peerSet = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();

                threadEventParameter.Clear();
                //广播事件
                threadEventParameter.Add((byte)ParameterCode.OccupiedUnit, occupiedUnitJson);
                QueueThreadEvent(peerSet, EventCode.OccupiedResourceUnit, threadEventParameter);
            }else
                OpResponse.ReturnCode = (short)ReturnCode.Fail;
            OpResponse.Parameters = ResponseData;
            OpResponse.OperationCode = operationRequest.OperationCode;
            peer.SendOperationResponse(OpResponse, sendParameters);
        }
    }
}
