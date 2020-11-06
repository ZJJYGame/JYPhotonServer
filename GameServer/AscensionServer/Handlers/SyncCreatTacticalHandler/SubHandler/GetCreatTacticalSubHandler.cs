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
            Utility.Debug.LogInfo("yzqData收到的创建阵法数据" + tacticJson);
            if (!result)
            {
                if (Exist)
                {
                    if (RedisExist)
                    {
                        if (roletactical.Count >= 3)
                        {
                            GameManager.CustomeModule<TacticalDeploymentManager>().TryRemove(tacticalDTO.RoleID, roletactical[0].ID);
                            GameManager.CustomeModule<TacticalDeploymentManager>().SendAllLevelRoleTactical(roletactical[0], ReturnCode.Fail);
                            roletactical.Remove(roletactical[0]);
                        }
                    }else
                        roletactical = new List<TacticalDTO>();
                    roletactical.Add(tacticalDTO);
                    RedisHelper.Hash.HashSet("TacticalDTO" + tacticalDTO.RoleID, tacticalDTO.RoleID.ToString(), roletactical);
                    #region 广播给当前场景所有人
                    GameManager.CustomeModule<TacticalDeploymentManager>().SendAllLevelRoleTactical(tacticalDTO, ReturnCode.Success );
                    #endregion
                    #region Redis对于时间的记录
                    RedisHelper.String.StringSet("Tactical" + "$" + 0 + "$" + tacticalDTO.ID, tacticalDTO.RoleID.ToString(), new TimeSpan(0, 0, 0, tacticalDTO.Duration));
                    RedisManager.Instance.AddKeyExpireListener("Tactical" + "$" + 0 + "$" + tacticalDTO.ID, GameManager.CustomeModule<TacticalDeploymentManager>().RedisDeleteCaback);
                    #endregion
                    SetResponseParamters(() =>
                    {
                        operationResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
                else
                {
                    RedisHelper.Hash.HashSet("TacticalDTO" + tacticalDTO.RoleID, tacticalDTO.RoleID.ToString(), roletactical);
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
