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
    public class OccupiedResourceUnitHandler:Handler
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
            var occupiedUnitObj = Utility.Json.ToObject<OccupiedUnitDTO>(occupiedUnitJson);
            var result= AscensionServer.Instance.OccupiedResUnit(occupiedUnitObj.GlobalID, occupiedUnitObj.ResID);
            if (result)
            {
                OpResponse.ReturnCode = (short)ReturnCode.Success;
            }else
                OpResponse.ReturnCode = (short)ReturnCode.Fail;
            OpResponse.Parameters = ResponseData;
            peer.SendOperationResponse(OpResponse, sendParameters);
        }
    }
}
