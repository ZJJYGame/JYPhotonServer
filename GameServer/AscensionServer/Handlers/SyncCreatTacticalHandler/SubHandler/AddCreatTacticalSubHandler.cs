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
    class AddCreatTacticalSubHandler : SyncCreatTacticalSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string tacticJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.CreatTactical));
            var tacticObj = Utility.Json.ToObject<TacticalDTO>(tacticJson);
           var id= GameEntry.TacticalDeploymentManager.GetExpendTacticalID();
            tacticObj.ID = id;
        var result= GameEntry.TacticalDeploymentManager.IsCreatTactic();
            if (result)
            {
                var Exits= GameEntry.TacticalDeploymentManager.TacticalCreateAdd(tacticObj);
                if (Exits)
                {
                    SetResponseParamters(() =>
                    {
                        operationResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
                else
                    SetResponseParamters(() =>
                    {
                        operationResponse.ReturnCode = (short)ReturnCode.Fail;
                    });
            }
            return operationResponse;
        }
    }
}


