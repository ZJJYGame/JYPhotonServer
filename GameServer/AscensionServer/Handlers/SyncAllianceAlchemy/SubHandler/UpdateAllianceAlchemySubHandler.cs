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
    public class UpdateAllianceAlchemySubHandler : SyncAllianceAlchemySubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Update;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string allianceAlchemyJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAlliance));
            var allianceCaveObj = Utility.Json.ToObject<RoleAllianceDTO>(allianceAlchemyJson);

            string roleAssetsJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAssets));

            string AllianceAlchemyJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAllianceAlchemy));
            AllianceAlchemyNumDTO allianceAlchemyNumDTO = Utility.Json.ToObject<AllianceAlchemyNumDTO>(AllianceAlchemyJson);

            var redisKey = RedisData.ReidsDataProcessing.InsertName("AllianceAlchemyNum", allianceCaveObj.RoleID);
            var content = RedisData.ReidsDataProcessing.GetData(redisKey);

            var roleAssetsObj = Utility.Json.ToObject<RoleAssetsDTO>(roleAssetsJson);
            Utility.Debug.LogError("收到的兑换弹药的请求数据"+ roleAssetsJson+ allianceAlchemyJson);
            var roleallianceTemp = AlliancelogicManager.Instance.GetNHCriteria<RoleAlliance>("RoleID", allianceCaveObj.RoleID);
            var roleAssetsTemp = AlliancelogicManager.Instance.GetNHCriteria<RoleAssets>("RoleID", roleAssetsObj.RoleID);
            if (roleallianceTemp != null && roleAssetsTemp != null)
            {
                if (roleallianceTemp.Reputation >= allianceCaveObj.Reputation && roleAssetsTemp.SpiritStonesLow >= roleAssetsObj.SpiritStonesLow)
                {
                    roleallianceTemp.Reputation -= allianceCaveObj.Reputation;
                    roleAssetsTemp.SpiritStonesLow -= roleAssetsObj.SpiritStonesLow;

                    NHibernateQuerier.Update(roleallianceTemp);
                    NHibernateQuerier.Update(roleAssetsTemp);
                    RedisHelper.Hash.HashSet<RoleAssets>("RoleAssets", roleAssetsObj.RoleID.ToString(), roleAssetsTemp);
                    var Role = AlliancelogicManager.Instance.GetNHCriteria<Role>("RoleID", roleallianceTemp.RoleID);
                    RoleAllianceDTO roleAllianceDTO = new RoleAllianceDTO() { RoleID = roleallianceTemp.RoleID, AllianceID = roleallianceTemp.AllianceID, JoinOffline = roleallianceTemp.JoinOffline, AllianceJob = roleallianceTemp.AllianceJob, ApplyForAlliance = Utility.Json.ToObject<List<int>>(roleallianceTemp.ApplyForAlliance), JoinTime = roleallianceTemp.JoinTime, Reputation = roleallianceTemp.Reputation, ReputationHistroy = roleallianceTemp.ReputationHistroy, ReputationMonth = roleallianceTemp.ReputationMonth, RoleName = roleallianceTemp.RoleName,RoleLevel= Role.RoleLevel };
                    if (string.IsNullOrEmpty(content))
                    {
                        #region 测试完成替换
                        //int h = 23 - DateTime.Now.Hour;
                        //int m = 59 - DateTime.Now.Minute;
                        //int s = 60 - DateTime.Now.Second;
                        //await RedisHelper.String.StringSetAsync(redisKey, AllianceAlchemyJson, new TimeSpan(0, h, m, s));
                        #endregion
                        RedisHelper.String.StringSet(redisKey, AllianceAlchemyJson, new TimeSpan(0, 0, 0, 60));
                    }
                    else
                    {
                        var alchemyDict = Utility.Json.ToObject<AllianceAlchemyNumDTO>(content).AlchemyNum;
                        foreach (var item in allianceAlchemyNumDTO.AlchemyNum)
                        {
                            if (alchemyDict.ContainsKey(item.Key))
                            {
                                alchemyDict[item.Key] += item.Value;
                            }
                            else
                                alchemyDict.Add(item.Key,item.Value);
                        }
                        allianceAlchemyNumDTO.AlchemyNum = alchemyDict;
                        AllianceAlchemyJson = Utility.Json.ToJson(allianceAlchemyNumDTO);
                        RedisHelper.String.StringSet(redisKey, AllianceAlchemyJson, RedisHelper.KeyTimeToLiveAsync(redisKey).Result);
                    }
                    SetResponseParamters(() =>
                    {
                        Utility.Debug.LogError("发送回去的兑换弹药的请求数据" + Utility.Json.ToJson(roleAssetsTemp));
                        subResponseParameters.Add((byte)ParameterCode.RoleAlliance, Utility.Json.ToJson(roleAllianceDTO));
                        subResponseParameters.Add((byte)ParameterCode.RoleAllianceAlchemy, AllianceAlchemyJson);
                        operationResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
                else
                {
                    SetResponseParamters(() =>
                    {
                        operationResponse.ReturnCode = (short)ReturnCode.Fail;
                    });
                }
            }
            else
            {
                SetResponseParamters(() =>
                {
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            return operationResponse;
        }
    }
}


