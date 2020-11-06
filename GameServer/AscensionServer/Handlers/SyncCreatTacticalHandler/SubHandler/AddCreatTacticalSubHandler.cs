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
            Dictionary<int, List<TacticalDTO>> tacticDict = new Dictionary<int, List<TacticalDTO>>();
            //GameManager.CustomeModule<TacticalDeploymentManager>().GetRoleTactic(tacticObj.RoleID,out tacticDict);
           var id= GameManager.CustomeModule<TacticalDeploymentManager>().GetExpendTacticalID();
            tacticObj.ID = id;
        var result= GameManager.CustomeModule<TacticalDeploymentManager>().IsCreatTactic(out TacticalDeploymentDTO tacticalDeployment);
            if (result)
            {
                SetResponseParamters(() =>
                {
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            else
            {
                SetResponseParamters(() => {
                    operationResponse.ReturnCode = (short)ReturnCode.Success;
                });
                GameManager.CustomeModule<TacticalDeploymentManager>().AddTacTical(tacticObj);
            }
            return operationResponse;
        }
    }
}
