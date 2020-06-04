using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;

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
            string roleGFJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.GongFaExp));
            var RoleGongFatemp = Utility.ToObject<RoleGongFa>(roleGFJson);

            NHCriteria nHCriteriaGf = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", RoleGongFatemp.RoleID);
            RoleGongFa roleGongFa = Singleton<NHManager>.Instance.CriteriaGet<RoleGongFa>(nHCriteriaGf);
            //AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>同步功法进来了>>>>>>>>>" + roleGongFa.RoleID);
            #region
            //if (string.IsNullOrEmpty(roleGongFa.GongFaIDArray))
            //{
            //    SubDict.Add((byte)ParameterCode.Account, Utility.ToJson(new List<string>()));
            //    Owner.ResponseData.Add((byte)OperationCode.SubOpCodeData, Utility.ToJson(SubDict));
            //    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Empty;
            //}
            //else
            //{
            //    var rolGFObj = Singleton<NHManager>.Instance.CriteriaGet<RoleGongFa>(nHCriteriaGf);
            //    string gongfalist = roleGongFa.GongFaIDArray;
            //    List<string> roleGFIDlist;
            //    List<GongFa> gongFaslist = new List<GongFa>();
            //    List<NHCriteria> nHCriterias = new List<NHCriteria>();
            //    if (!string.IsNullOrEmpty(gongfalist))
            //    {
            //        roleGFIDlist = new List<string>();
            //        roleGFIDlist = Utility.ToObject<List<string>>(gongfalist);
            //        for (int i = 0; i < roleGFIDlist.Count; i++)
            //        {
            //            NHCriteria nHCriteriatemp = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", int.Parse(roleGFIDlist[i]));
                       
            //            GongFa gongFatemp = Singleton<NHManager>.Instance.CriteriaGet<GongFa>(nHCriteriatemp);
            //            gongFaslist.Add(gongFatemp);
            //            nHCriterias.Add(nHCriteriatemp);
            //            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>同步功法进来了>>>>>>>>>" + Utility.ToJson(gongFaslist));
            //        }
            //    }

            //    SubDict.Add((byte)ParameterCode.GongFaExp, Utility.ToJson(gongFaslist));
            //   // Owner.ResponseData.Add((byte)ParameterCode.GongFaExp, Utility.ToJson(gongFaslist));
            //    //Owner.OpResponse.Parameters = Owner.ResponseData;
            //    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
            //    Singleton<ReferencePoolManager>.Instance.Despawns(nHCriterias);

            //}
#endregion
            Utility.Assert.NotNull(roleGongFa.GongFaIDArray, () =>
            {
                var rolGFObj = Singleton<NHManager>.Instance.CriteriaGet<RoleGongFa>(nHCriteriaGf);
                string gongfalist = roleGongFa.GongFaIDArray;
                List<string> roleGFIDlist;
                List<GongFa> gongFaslist = new List<GongFa>();
                List<NHCriteria> nHCriterias = new List<NHCriteria>();
                Utility.Assert.NotNull(gongfalist, () =>
                 {
                     roleGFIDlist = new List<string>();
                     roleGFIDlist = Utility.ToObject<List<string>>(gongfalist);
                     for (int i = 0; i < roleGFIDlist.Count; i++)
                     {
                         NHCriteria nHCriteriatemp = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", int.Parse(roleGFIDlist[i]));

                         GongFa gongFatemp = Singleton<NHManager>.Instance.CriteriaGet<GongFa>(nHCriteriatemp);
                         gongFaslist.Add(gongFatemp);
                         nHCriterias.Add(nHCriteriatemp);
                         AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>同步功法进来了>>>>>>>>>" + Utility.ToJson(gongFaslist));
                     }
                 });
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.GongFaExp, Utility.ToJson(gongFaslist));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
                Singleton<ReferencePoolManager>.Instance.Despawns(nHCriterias);
            },
            () => SetResponseData(() =>
            {
                SubDict.Add((byte)ParameterCode.RoleList, Utility.ToJson(new List<string>()));
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Empty;
            }));

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaGf);
        }
        }
    }

