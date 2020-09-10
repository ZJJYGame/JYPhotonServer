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
using NHibernate.Criterion;
namespace AscensionServer
{
    public class VerifyImmortalsAllianceHandler : SyncImmortalsAllianceSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Verify;
            base.OnInitialization();
        }


        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string alliancestatusJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ImmortalsAlliance));
            var alliancestatusObj = Utility.Json.ToObject<AllianceStatusDTO>
                (alliancestatusJson);


            List<AllianceStatusDTO> allianceStatusDTOs = new List<AllianceStatusDTO>();

            NHCriteria nHCriteriaAllianceID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceName", alliancestatusObj.AllianceName);

            var allianceIDObj = ConcurrentSingleton<NHManager>.Instance.CriteriaLikeAsync<AllianceStatus>(nHCriteriaAllianceID, MatchMode.Anywhere).Result;

            Utility.Debug.LogError("1查询获得MySQL的数据" + allianceIDObj[0].AllianceName + "%%" + allianceIDObj[2].AllianceName + "**" + "查询获得MySQL的数据长度" + allianceIDObj.Count);
                if (allianceIDObj.Count == 0)
                {
                    NHCriteria nHCriteriaAllianceName = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", alliancestatusObj.AllianceName);
                    var allianceNameObj = ConcurrentSingleton<NHManager>.Instance.CriteriaLikeAsync<AllianceStatus>(nHCriteriaAllianceName, MatchMode.Anywhere).Result;
                    if (allianceNameObj.Count > 0)
                    {
                        Utility.Debug.LogError("2查询获得MySQL的数据" + allianceIDObj[0].AllianceName + "%%" + allianceIDObj[2].AllianceName + "**" + "查询获得MySQL的数据长度" + allianceIDObj.Count);
                        for (int i = 0; i < allianceNameObj.Count; i++)
                        {
                            AllianceStatusDTO allianceStatusDTO = GameManager.ReferencePoolManager.Spawn<AllianceStatusDTO>();
                            allianceStatusDTO.ID = allianceNameObj[i].ID;
                            allianceStatusDTO.AllianceLevel = allianceNameObj[i].AllianceLevel;
                            allianceStatusDTO.AllianceMaster = allianceNameObj[i].AllianceMaster;
                            allianceStatusDTO.AllianceName = allianceNameObj[i].AllianceName;
                            allianceStatusDTO.AllianceNumberPeople = allianceNameObj[i].AlliancePeopleMax;
                            allianceStatusDTO.Manifesto = allianceNameObj[i].Manifesto;
                            allianceStatusDTO.Popularity = allianceNameObj[i].Popularity;
                            allianceStatusDTOs.Add(allianceStatusDTO);
                        }
                        SetResponseData(() =>
                        {
                            Utility.Debug.LogInfo("发送的所有仙盟列表" + Utility.Json.ToJson(allianceStatusDTOs));
                            SubDict.Add((byte)ParameterCode.ImmortalsAlliance, Utility.Json.ToJson(allianceStatusDTOs));
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                        });
                    }
                    else
                    {
                        SetResponseData(() =>
                        {
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                        });
                    }
                }
                else
                {
                    for (int i = 0; i < allianceIDObj.Count; i++)
                    {
                        AllianceStatusDTO allianceStatusDTO = GameManager.ReferencePoolManager.Spawn<AllianceStatusDTO>();
                        allianceStatusDTO.ID = allianceIDObj[i].ID;
                        allianceStatusDTO.AllianceLevel = allianceIDObj[i].AllianceLevel;
                        allianceStatusDTO.AllianceMaster = allianceIDObj[i].AllianceMaster;
                        allianceStatusDTO.AllianceName = allianceIDObj[i].AllianceName;
                        allianceStatusDTO.AllianceNumberPeople = allianceIDObj[i].AlliancePeopleMax;
                        allianceStatusDTO.Manifesto = allianceIDObj[i].Manifesto;
                        allianceStatusDTO.Popularity = allianceIDObj[i].Popularity;
                        allianceStatusDTOs.Add(allianceStatusDTO);
                    }
                    SetResponseData(() =>
                    {
                        Utility.Debug.LogInfo("发送的所有仙盟列表" + Utility.Json.ToJson(allianceStatusDTOs));
                        SubDict.Add((byte)ParameterCode.ImmortalsAlliance, Utility.Json.ToJson(allianceStatusDTOs));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
         
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaAllianceID);
        }
    }
}

