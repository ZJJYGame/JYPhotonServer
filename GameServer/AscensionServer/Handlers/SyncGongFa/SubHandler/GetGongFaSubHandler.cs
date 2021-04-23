using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using FluentNHibernate.Testing.Values;
using NHibernate.Mapping;
using Cosmos;
using RedisDotNet;
namespace AscensionServer
{
    public class GetGongFaSubHandler : SyncGongFaSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;
        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {

            var dict = operationRequest.Parameters;
            string roleGFJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.GongFa));
            var roleGongFaObj = Utility.Json.ToObject<List<int>>(roleGFJson);
            List<CultivationMethodDTO> gongFaIdList;
            Dictionary<int, List<CultivationMethodDTO>> gongFaDic;
            Utility.Debug.LogInfo(">>>>>>>>>>>>>>>>>同步功法进来了>>>>>>>>>" + roleGongFaObj.Count);



                if (roleGongFaObj.Count != 0)
                {
                    gongFaDic = new Dictionary<int, List<CultivationMethodDTO>>();
                    foreach (var roleId in roleGongFaObj)
                    {
                        NHCriteria nHCriteriaGongFa = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleId);
                        bool exist = NHibernateQuerier.Verify<RoleGongFa>(nHCriteriaGongFa);
                        gongFaIdList = new List<CultivationMethodDTO>();
                        if (exist)
                        {
                            var roleIdArray = NHibernateQuerier.CriteriaSelect<RoleGongFa>(nHCriteriaGongFa);
                            if (!string.IsNullOrEmpty(roleIdArray.GongFaIDDict))
                            {
                                foreach (var gongFaId in Utility.Json.ToObject<Dictionary<int, int>>(roleIdArray.GongFaIDDict))
                                {
                                    NHCriteria nHCriteriaGongFaId = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", gongFaId.Key);
                                    var gongFaIdArray = NHibernateQuerier.CriteriaSelect<CultivationMethod>(nHCriteriaGongFaId);
                                var gongfaTemp= CosmosEntry.ReferencePoolManager.Spawn<CultivationMethodDTO>();
                                gongfaTemp.CultivationMethodExp = gongFaIdArray.CultivationMethodExp;
                                gongfaTemp.CultivationMethodLevel = gongFaIdArray.CultivationMethodLevel;
                                gongfaTemp.CultivationMethodLevelSkillArray = Utility.Json.ToObject<List<int>>(gongFaIdArray.CultivationMethodLevelSkillArray);
                                gongfaTemp.CultivationMethodID = gongFaIdArray.CultivationMethodID;

                                gongFaIdList.Add(gongfaTemp);
                                }
                                gongFaDic.Add(roleId, gongFaIdList);
                            }
                            else
                                gongFaDic.Add(roleId, new List<CultivationMethodDTO>());
                        }
                        else
                        {
                            NHibernateQuerier.Insert(new RoleGongFa() { RoleID = roleId });
                            gongFaDic.Add(roleId, new List<CultivationMethodDTO>());
                        }
                        CosmosEntry.ReferencePoolManager.Despawns(nHCriteriaGongFa);
                    }
                SetResponseParamters(() => {
                    operationResponse.Parameters = subResponseParameters;
                    subResponseParameters.Add((byte)ParameterCode.GongFa, Utility.Json.ToJson(gongFaDic));
                    operationResponse.ReturnCode = (short)ReturnCode.Success;
                });
                }
                else
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
            return operationResponse;
        }
    }
}



