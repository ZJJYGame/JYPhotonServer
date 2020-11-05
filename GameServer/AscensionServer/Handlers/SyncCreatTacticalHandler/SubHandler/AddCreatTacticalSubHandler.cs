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
            Utility.Debug.LogInfo("yzqData创建阵法"+ tacticJson);

            Dictionary<int, List<TacticalDTO>> tacticDict = new Dictionary<int, List<TacticalDTO>>();
            //GameManager.CustomeModule<TacticalDeploymentManager>().GetRoleTactic(tacticObj.RoleID,out tacticDict);
           var id= GameManager.CustomeModule<TacticalDeploymentManager>().GetExpendTacticalID();
            tacticObj.ID = id;
        var result=    GameManager.CustomeModule<TacticalDeploymentManager>().IsCreatTactic(out TacticalDeploymentDTO tacticalDeployment);
            #region 待删
            //if (result)
            //{
            //    if (Exist)
            //    {
            //        if (roletactical.Count >= 3)
            //        {
            //            roletactical.Remove(roletactical[0]);
            //        }
            //    }
            //    roletactical.Add(tacticObj);
            //    RedisHelper.Hash.HashSet<List<TacticalDTO>>("TacticalDTO" + tacticObj.RoleID, tacticObj.RoleID.ToString(), roletactical);
            //    GameManager.CustomeModule<TacticalDeploymentManager>().AddTacTical(tacticObj);
            //    Utility.Debug.LogInfo("yzqData创建阵法成功>>>>>>>" + Utility.Json.ToJson(tacticObj));
            //}
            //else
            //{
            //    GameManager.CustomeModule<TacticalDeploymentManager>().TryAddFirst(tacticObj);
            //}
            #endregion
            if (result)
            {
                SetResponseParamters(() => {
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            else
                GameManager.CustomeModule<TacticalDeploymentManager>().AddTacTical(tacticObj);
            return operationResponse;
        }
    }
}
