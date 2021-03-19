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
    public class UpdateAllianceSigninSubHandler : SyncAllianceSigninSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Update;
        public override OperationResponse EncodeMessage(OperationRequest operationRequest)

        {
            var dict = operationRequest.Parameters;
            string allianceSigninJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AllianceSignin));
            var allianceSigninObj = Utility.Json.ToObject<AllianceSigninDTO>(allianceSigninJson);

            var roleallianceTemp = AlliancelogicManager.Instance.GetNHCriteria<RoleAlliance>("RoleID", allianceSigninObj.RoleID);
            var allianceTemp = AlliancelogicManager.Instance.GetNHCriteria<AllianceStatus>("ID", allianceSigninObj.AllianceID);

            var allianceConstructionTemp = AlliancelogicManager.Instance.GetNHCriteria<AllianceConstruction>("AllianceID", allianceSigninObj.AllianceID);
            var Role = AlliancelogicManager.Instance.GetNHCriteria<Role>("RoleID", roleallianceTemp.RoleID);

            GameEntry. DataManager.TryGetValue<List<AllianceSigninData>>(out var allianceSigninList);
            var content = RedisHelper.KeyExistsAsync("AllianceSigninDTO"+ allianceTemp.ID).Result;
           // Utility.Debug.LogError("yzqs角色数据数据" + Utility.Json.ToJson(allianceSigninList));
            var alliancesignindata=  allianceSigninList.Find(t=>t.Role_Level[0]<= Role.RoleLevel&& t.Role_Level[1] >=Role.RoleLevel);
         //   Utility.Debug.LogError("yzq收到的签到数据数据" +Utility.Json.ToJson(alliancesignindata));
            if (!content)
            {
              //  Utility.Debug.LogError("yzq收到的签到数据数据" + allianceSigninJson);
                if (alliancesignindata != null)
                {
                    List<string> signinList = new List<string>();
                    if (roleallianceTemp != null && allianceTemp != null && allianceConstructionTemp != null)
                    {
                       // Utility.Debug.LogError("yzq2收到的签到数据数据" + Utility.Json.ToJson(allianceSigninJson));
                        roleallianceTemp.Reputation += alliancesignindata.Role_Contribution;
                        roleallianceTemp.ReputationHistroy += alliancesignindata.Role_Contribution;
                        roleallianceTemp.ReputationMonth += alliancesignindata.Role_Contribution;

                        allianceTemp.Popularity += alliancesignindata.Alliance_Popularity;
                        allianceConstructionTemp.AllianceAssets += alliancesignindata.Alliance_Spirit_Stone;

                        RoleAllianceDTO roleAllianceDTO = new RoleAllianceDTO() { AllianceID = roleallianceTemp.AllianceID, AllianceJob = roleallianceTemp.AllianceJob, JoinTime = roleallianceTemp.JoinTime, ApplyForAlliance = Utility.Json.ToObject<List<int>>(roleallianceTemp.ApplyForAlliance), JoinOffline = roleallianceTemp.JoinOffline, Reputation = roleallianceTemp.Reputation, ReputationHistroy = roleallianceTemp.ReputationHistroy, ReputationMonth = roleallianceTemp.ReputationMonth, RoleID = roleallianceTemp.RoleID, RoleName = roleallianceTemp.RoleName, RoleLevel = Role.RoleLevel };

                        signinList.Add(Utility.Json.ToJson(roleAllianceDTO));
                        signinList.Add(Utility.Json.ToJson(allianceTemp));
                        signinList.Add(Utility.Json.ToJson(allianceConstructionTemp));
                        NHibernateQuerier.Update(roleallianceTemp);
                        NHibernateQuerier.Update(allianceTemp);
                        NHibernateQuerier.Update(allianceConstructionTemp);
                        int h = 23 - DateTime.Now.Hour;
                        int m = 59 - DateTime.Now.Minute;
                        int s = 60 - DateTime.Now.Second;
                        allianceSigninObj.IsSignin = true;
                        RedisHelper.String.StringSet("AllianceSigninDTO"+ allianceTemp.ID, allianceSigninObj, new TimeSpan(0, h, m, s));
                        SetResponseParamters(() =>
                        {
                            Utility.Debug.LogError("yzq发送回去的签到数据数据" + Utility.Json.ToJson(signinList));
                            subResponseParameters.Add((byte)ParameterCode.AllianceSignin, Utility.Json.ToJson(signinList));
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
            }
            else
            {
                var allianceSignin = RedisHelper.String.StringGet("AllianceSigninDTO" + allianceTemp.ID);
                SetResponseParamters(() =>
                {
                    subResponseParameters.Add((byte)ParameterCode.AllianceSignin, allianceSignin);
                    operationResponse.ReturnCode = (short)ReturnCode.ItemAlreadyExists;
                });
            }
            return operationResponse;
        }
    }
}


