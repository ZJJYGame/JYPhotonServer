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
            var result = GameManager.CustomeModule<TacticalDeploymentManager>().IsCreatTactic(out TacticalDeploymentDTO tacticalDeployment);
            //获取临时储存的阵法并移除
            var Exist = GameManager.CustomeModule<TacticalDeploymentManager>().GetRemoveTacTical(tacticObj.RoleID, out TacticalDTO tacticalDTO);
            var RedisExist = GameManager.CustomeModule<TacticalDeploymentManager>().GetRoleTactic(tacticObj.RoleID, out List<TacticalDTO> roletactical);
            if (!result)
            {
                if (Exist)
                {
                    if (RedisExist)
                    {
                        if (roletactical.Count >= 3)
                        {
                            roletactical.Remove(roletactical[0]);
                            GameManager.CustomeModule<TacticalDeploymentManager>().TryRemove(tacticObj.RoleID, roletactical[0].ID);
                        }
                    }
                    roletactical = new List<TacticalDTO>();
                    roletactical.Add(tacticalDTO);
                    RedisHelper.Hash.HashSet("TacticalDTO" + tacticObj.RoleID, tacticObj.RoleID.ToString(), roletactical);
                    #region 广播给当前场景所有人
                    GameManager.CustomeModule<TacticalDeploymentManager>().SendAllLevelRoleTactical(tacticObj);
                    #endregion
                    SetResponseParamters(() =>
                    {
                        operationResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
                else
                {
                    RedisHelper.Hash.HashSet("TacticalDTO" + tacticObj.RoleID, tacticObj.RoleID.ToString(), roletactical);
                }
            }
            else
            {
                SetResponseParamters(() => {
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            return operationResponse;
        }
    }
}
