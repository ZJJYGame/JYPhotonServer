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
using Protocol;
namespace AscensionServer
{
    /// <summary>
    /// 占用资源单位请求；
    /// 例如怪物被占用，则其他玩家无法触发战斗；
    /// 矿石被占用，则其他玩家无法拾取
    /// </summary>
    public class OccupiedResourceUnitHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.OccupiedResourceUnit; } }
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            responseParameters.Clear();
            var occupiedUnitJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.OccupiedUnit));
            Utility.Debug.LogInfo("请求资源数据  :  " + occupiedUnitJson);
            var occupiedUnitObj = Utility.Json.ToObject<OccupiedUnitDTO>(occupiedUnitJson);
            var result =GameManager.CustomeModule< GameResourceManager>().OccupiedResUnit(occupiedUnitObj);
            if (result)
            {
                operationResponse.ReturnCode = (short)ReturnCode.Success;

                ResourceUnitSetDTO currentDictObj = null;
                if (GameManager.CustomeModule<GameResourceManager>().ResUnitSetDict.TryGetValue(occupiedUnitObj.GlobalID,out currentDictObj))
                {

                    ResourceUnitDTO resourceUnitDTO = null;
                    if (currentDictObj.ResUnitDict.TryGetValue(occupiedUnitObj.ResID, out resourceUnitDTO))
                        resourceUnitDTO.Occupied = result;
                }
                //var peerSet = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
                //threadEventParameter.Clear();
                //广播事件
                //threadEventParameter.Add((byte)ParameterCode.OccupiedUnit, occupiedUnitJson);
                //QueueThreadEvent(peerSet, EventCode.OccupiedResourceUnit, threadEventParameter);

                var levelmanager = GameManager.CustomeModule<LevelManager>();
                OperationData operationData = new OperationData();
                operationData.DataMessage = Utility.Json.ToJson(occupiedUnitObj);
                GameManager.CustomeModule<GameResourceManager>().OccupiedUnitSetCache.Clear();
                levelmanager.SendMsg2AllLevelRoleS2C(0,operationData);
            }
            else
                operationResponse.ReturnCode = (short)ReturnCode.Fail;
            operationResponse.Parameters = responseParameters;
            operationResponse.OperationCode = operationRequest.OperationCode;
            return operationResponse;
        }
    }
}
