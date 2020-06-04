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
            string roleGFJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.GongFa));
            var roleGongFaObj = Utility.ToObject<RoleGongFa>(roleGFJson);
            NHCriteria nHCriteriaGongFa = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleGongFaObj.RoleID);
            RoleGongFa roleGongFa = Singleton<NHManager>.Instance.CriteriaGet<RoleGongFa>(nHCriteriaGongFa);
            Utility.Assert.IsNull(roleGongFa, () => {
                roleGongFaObj = Singleton<NHManager>.Instance.Add(new RoleGongFa() { RoleID = roleGongFaObj.RoleID });
            });
            Utility.Assert.NotNull(roleGongFa, () =>
            {
                roleGongFaObj = Singleton<NHManager>.Instance.CriteriaGet<RoleGongFa>(nHCriteriaGongFa);
                string gongFaIDList = roleGongFa.GongFaIDArray;
                List<string> roleGFIDList;
                List<GongFa> gongFaList = new List<GongFa>();
                List<NHCriteria> nHCriteriaList = new List<NHCriteria>();
                Utility.Assert.NotNull(gongFaIDList, () =>
                 {
                     roleGFIDList = new List<string>();
                     roleGFIDList = Utility.ToObject<List<string>>(gongFaIDList);
                     for (int i = 0; i < roleGFIDList.Count; i++)
                     {
                         NHCriteria nHCriteriaTemp = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", int.Parse(roleGFIDList[i]));
                         GongFa gongFaTemp = Singleton<NHManager>.Instance.CriteriaGet<GongFa>(nHCriteriaTemp);
                         gongFaList.Add(gongFaTemp);
                         nHCriteriaList.Add(nHCriteriaTemp);
                         AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>同步功法进来了>>>>>>>>>" + Utility.ToJson(gongFaList));
                     }
                 });
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ObjectParameterCode.GongFa, Utility.ToJson(gongFaList));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
                Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaList);
            },
            () => SetResponseData(() =>
            {
                SubDict.Add((byte)ParameterCode.RoleList, Utility.ToJson(new List<string>()));
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Empty;
            }));
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaGongFa);
        }
        }
    }

