using AscensionProtocol;
using Cosmos;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
namespace AscensionServer
{
    /// <summary>
    /// 占用资源单位请求；
    /// 例如怪物被占用，则其他玩家无法触发战斗；
    /// 矿石被占用，则其他玩家无法拾取
    /// </summary>
    public class OccupiedResourceUnitHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.TakeUpResource; } }
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            responseParameters.Clear();
            var occupiedUnitJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.OccupiedUnit));
            Utility.Debug.LogInfo("请求资源数据  :  " + occupiedUnitJson);
            var occupiedUnitObj = Utility.Json.ToObject<OccupiedUnitDTO>(occupiedUnitJson);
            bool result=false;
            if (result)
            {
                operationResponse.ReturnCode = (short)ReturnCode.Success;

                ResourceUnitSetDTO currentDictObj = null;
                //if (GameManager.CustomeModule<MapResourceManager>().ResUnitSetDict.TryGetValue(occupiedUnitObj.GlobalID,out currentDictObj))
                //{

                //    ResourceUnitDTO resourceUnitDTO = null;
                //    if (currentDictObj.ResUnitDict.TryGetValue(occupiedUnitObj.ResID, out resourceUnitDTO))
                //        resourceUnitDTO.Occupied = result;
                //}
                var levelmanager = GameEntry.LevelManager;
                OperationData operationData = new OperationData();
                operationData.DataMessage = Utility.Json.ToJson(occupiedUnitObj);
                //GameManager.CustomeModule<MapResourceManager>().OccupiedUnitSetCache.Clear();
                levelmanager.SendMessageToLevelS2C(LevelTypeEnum.Adventure,0, operationData);
            }
            else
                operationResponse.ReturnCode = (short)ReturnCode.Fail;
            operationResponse.Parameters = responseParameters;
            operationResponse.OperationCode = operationRequest.OperationCode;
            return operationResponse;
        }
    }
}


