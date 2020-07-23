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
namespace AscensionServer
{
    public class GetGongFaSubHandler : SyncGongFaSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string roleGFJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.GongFa));
            var roleGongFaObj = Utility.Json.ToObject<List<int>>(roleGFJson);
            List<CultivationMethod> gongFaIdList;
            Dictionary<int, List<CultivationMethod>> gongFaDic;
            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>同步功法进来了>>>>>>>>>" + roleGongFaObj.Count);
            if (roleGongFaObj.Count != 0)
            {
                gongFaDic = new Dictionary<int, List<CultivationMethod>>();
                foreach (var roleId in roleGongFaObj)
                {
                    NHCriteria nHCriteriaGongFa = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleId);
                    bool exist = ConcurrentSingleton<NHManager>.Instance.Verify<RoleGongFa>(nHCriteriaGongFa);
                    gongFaIdList = new List<CultivationMethod>();
                    if (exist)
                    {
                        var roleIdArray = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleGongFa>(nHCriteriaGongFa);
                        if (!string.IsNullOrEmpty(roleIdArray.GongFaIDArray))
                        {
                            foreach (var gongFaId in Utility.Json.ToObject<Dictionary<int, int>>(roleIdArray.GongFaIDArray))
                            {
                                NHCriteria nHCriteriaGongFaId = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", gongFaId.Key);
                                var gongFaIdArray = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<CultivationMethod>(nHCriteriaGongFaId);
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
                        ConcurrentSingleton<NHManager>.Instance.Insert(new RoleGongFa() { RoleID = roleId });
                        gongFaDic.Add(roleId, new List<CultivationMethod>());
                    }
                    ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaGongFa);
                }
                Owner.OpResponse.Parameters = Owner.ResponseData;
                Owner.ResponseData.Add((byte)ParameterCode.GongFa, Utility.Json.ToJson(gongFaDic));
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);

        }
    }
}

