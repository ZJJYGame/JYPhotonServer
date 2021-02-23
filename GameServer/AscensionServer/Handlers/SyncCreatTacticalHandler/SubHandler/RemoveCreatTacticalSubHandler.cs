using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
using RedisDotNet;
using StackExchange.Redis;

namespace AscensionServer
{
   public class RemoveCreatTacticalSubHandler: SyncCreatTacticalSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Remove;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string tacticJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.CreatTactical));
            var tacticObj = Utility.Json.ToObject<TacticalDTO>(tacticJson);

            //var result = GameEntry.TacticalDeploymentManager.TryRemove(0, tacticObj.ID);
            //if (result)
            //{
            //    GameEntry.TacticalDeploymentManager.SendAllLevelRoleTactical(tacticObj, ReturnCode.Fail);
            //}
            //else
            //    SetResponseParamters(() =>
            //    {
            //        operationResponse.ReturnCode = (short)ReturnCode.Fail;
            //    });
            return operationResponse;
        }
    }
}


