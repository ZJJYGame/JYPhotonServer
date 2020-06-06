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
            string roleGFJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.GongFa));
            var roleGongFaObj = Utility.ToObject<List<int>>(roleGFJson);
            List<GongFa> gongFaIdList;
            Dictionary<int, List<GongFa>> gongFaDic;
            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>同步功法进来了>>>>>>>>>" + roleGongFaObj.Count);
            if (roleGongFaObj.Count != 0)
            {
                gongFaDic = new Dictionary<int, List<GongFa>>();
                foreach (var roleId in roleGongFaObj)
                {
                    NHCriteria nHCriteriaGongFa = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleId);
                    bool exist = Singleton<NHManager>.Instance.Verify<RoleGongFa>(nHCriteriaGongFa);
                    gongFaIdList = new List<GongFa>();
                    if (exist)
                    {
                        var roleIdArray = Singleton<NHManager>.Instance.CriteriaGet<RoleGongFa>(nHCriteriaGongFa);
                        if (!string.IsNullOrEmpty(roleIdArray.GongFaIDArray))
                        {
                            foreach (var gongFaId in Utility.ToObject<List<int>>(roleIdArray.GongFaIDArray))
                            {
                                NHCriteria nHCriteriaGongFaId = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", gongFaId);
                                var gongFaIdArray = Singleton<NHManager>.Instance.CriteriaGet<GongFa>(nHCriteriaGongFaId);
                                //AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>同步功法进来了>>>>>>>>>" + gongFaIdArray.GongFaID);
                                gongFaIdList.Add(gongFaIdArray);
                            }
                            gongFaDic.Add(roleId, gongFaIdList);
                        }
                        else
                            gongFaDic.Add(roleId, new List<GongFa>());
                    }
                    else
                    {
                        Singleton<NHManager>.Instance.Add(new RoleGongFa() { RoleID = roleId });
                        gongFaDic.Add(roleId, new List<GongFa>());
                    }
                }
                Owner.OpResponse.Parameters = Owner.ResponseData;
                Owner.ResponseData.Add((byte)ObjectParameterCode.GongFa, Utility.ToJson(gongFaDic));
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

