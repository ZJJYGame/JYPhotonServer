using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
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
            var dict = ParseSubParameters(operationRequest);
            string roleGFJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.GongFa));
            var roleGongFaObj = Utility.Json.ToObject<List<int>>(roleGFJson);
            List<CultivationMethod> gongFaIdList;
            Dictionary<int, List<CultivationMethod>> gongFaDic;
            Utility.Debug.LogInfo(">>>>>>>>>>>>>>>>>同步功法进来了>>>>>>>>>" + roleGongFaObj.Count);

                if (roleGongFaObj.Count != 0)
                {
                    gongFaDic = new Dictionary<int, List<CultivationMethod>>();
                    foreach (var roleId in roleGongFaObj)
                    {
                        NHCriteria nHCriteriaGongFa = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleId);
                        bool exist = NHibernateQuerier.Verify<RoleGongFa>(nHCriteriaGongFa);
                        gongFaIdList = new List<CultivationMethod>();
                        if (exist)
                        {
                            var roleIdArray = NHibernateQuerier.CriteriaSelect<RoleGongFa>(nHCriteriaGongFa);
                            if (!string.IsNullOrEmpty(roleIdArray.GongFaIDArray))
                            {
                                foreach (var gongFaId in Utility.Json.ToObject<Dictionary<int, int>>(roleIdArray.GongFaIDArray))
                                {
                                    NHCriteria nHCriteriaGongFaId = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", gongFaId.Key);
                                    var gongFaIdArray = NHibernateQuerier.CriteriaSelect<CultivationMethod>(nHCriteriaGongFaId);
                                    //AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>同步功法进来了>>>>>>>>>" + gongFaIdArray.GongFaID);
                                    gongFaIdList.Add(gongFaIdArray);
                                }
                                gongFaDic.Add(roleId, gongFaIdList);
                            }
                            else
                                gongFaDic.Add(roleId, new List<CultivationMethod>());
                        }
                        else
                        {
                            NHibernateQuerier.Insert(new RoleGongFa() { RoleID = roleId });
                            gongFaDic.Add(roleId, new List<CultivationMethod>());
                        }
                        GameManager.ReferencePoolManager.Despawns(nHCriteriaGongFa);
                    }
                    operationResponse.Parameters = subResponseParameters;
                    subResponseParameters.Add((byte)ParameterCode.GongFa, Utility.Json.ToJson(gongFaDic));
                    operationResponse.ReturnCode = (short)ReturnCode.Success;
                }
                else
                {
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                }
            return operationResponse;
        }
    }
}

