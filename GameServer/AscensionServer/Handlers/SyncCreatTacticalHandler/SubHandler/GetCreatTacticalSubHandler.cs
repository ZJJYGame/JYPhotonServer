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
using Protocol;
namespace AscensionServer
{
   public class GetCreatTacticalSubHandler: SyncCreatTacticalSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string tacticJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.CreatTactical));
            var tacticObj = Utility.Json.ToObject<TacticalDTO>(tacticJson);
            //二次判断是否可以创建
            var result = GameManager.CustomeModule<TacticalDeploymentManager>().IsCreatTactic();
            if (result)
            {
                //获取临时储存的阵法并移除
                var Exist = GameManager.CustomeModule<TacticalDeploymentManager>().TryAddRemoveTactical(tacticObj.RoleID, out var tacticalDTO);
                if (Exist)
                {
                   GameManager.CustomeModule<TacticalDeploymentManager>().GetRoleTactic(tacticObj.RoleID, out List<TacticalDTO> roletactical);
                    GameManager.CustomeModule<TacticalDeploymentManager>().TryAddRoleAllTactical(tacticalDTO.RoleID, tacticalDTO);
                }
            }
            else
            {
                SetResponseParamters(() => {
                    operationResponse.ReturnCode = (short)ReturnCode.ItemAlreadyExists;
                });
            }
   return operationResponse;
        }
    }
}
