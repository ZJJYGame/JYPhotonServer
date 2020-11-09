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

            Utility.Debug.LogInfo("yzqData收到的创建阵法数据" + tacticJson);
            if (result)
            {
                //获取临时储存的阵法并移除
                var Exist = GameManager.CustomeModule<TacticalDeploymentManager>().TryAddRemoveTactical(tacticObj.RoleID, out var tacticalDTO);
                var RedisExist = GameManager.CustomeModule<TacticalDeploymentManager>().GetRoleTactic(tacticObj.RoleID, out List<TacticalDTO> roletactical);
                if (!RedisExist)
                {
                    roletactical = new List<TacticalDTO>();
                    if (Exist)
                    {
                        roletactical.Add(tacticalDTO);
                        RedisHelper.Hash.HashSet("TacticalDTO" + tacticalDTO.RoleID, tacticalDTO.RoleID.ToString(), roletactical);
                    }
                }             
            }
            else
            {
                SetResponseParamters(() => {
                    operationResponse.ReturnCode = (short)ReturnCode.ItemAlreadyExists;
                });
            }
            #region

            //if (!result)
            //{
            //    if (Exist)
            //    {
            //        if (RedisExist)
            //        {
            //            if (roletactical.Count >= 3)
            //            {
            //                GameManager.CustomeModule<TacticalDeploymentManager>().TryRemove(tacticalDTO.RoleID, roletactical[0].ID);
            //                GameManager.CustomeModule<TacticalDeploymentManager>().SendAllLevelRoleTactical(roletactical[0], ReturnCode.Fail);
            //                roletactical.Remove(roletactical[0]);
            //            }
            //        }
            //        else
            //            roletactical = new List<TacticalDTO>();
            //        roletactical.Add(tacticalDTO.TacticalDTO);
            //        RedisHelper.Hash.HashSet("TacticalDTO" + tacticalDTO.RoleID, tacticalDTO.RoleID.ToString(), roletactical);
            //        #region 广播给当前场景所有人
            //        GameManager.CustomeModule<TacticalDeploymentManager>().SendAllLevelRoleTactical(tacticalDTO.TacticalDTO, ReturnCode.Success);
            //        #endregion
            //        #region Redis对于时间的记录
            //        RedisHelper.String.StringSet("Tactical" + tacticalDTO.ID, tacticalDTO.RoleID.ToString(), new TimeSpan(0, 0, 0, 20));
            //        GameManager.CustomeModule<TacticalDeploymentManager>().RecordDelTactical.Add("JY_Tactical" + tacticalDTO.ID, new List<int>() { 0, tacticalDTO.ID });
            //        RedisManager.Instance.AddKeyExpireListener("Tactical" + tacticalDTO.ID, GameManager.CustomeModule<TacticalDeploymentManager>().RedisDeleteCaback);
            //        #endregion
            //        SetResponseParamters(() =>
            //        {
            //            operationResponse.ReturnCode = (short)ReturnCode.Success;
            //        });
            //    }
            //    else
            //    {
            //        RedisHelper.Hash.HashSet("TacticalDTO" + tacticalDTO.RoleID, tacticalDTO.RoleID.ToString(), roletactical);
            //    }
            //}
            //else
            //{
            //    SetResponseParamters(() => {
            //        operationResponse.ReturnCode = (short)ReturnCode.Fail;
            //    });
            //}
            #endregion
            return operationResponse;
        }
    }
}
