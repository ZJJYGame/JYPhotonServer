using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    public class GetMiShuSubHandle : SyncMiShuSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {

            var dict = ParseSubDict(operationRequest);
            string roleMSJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.MiShu));
            var roleMiShuObj = Utility.Json.ToObject<RoleMiShu>(roleMSJson);
            NHCriteria nHCriteriamishu = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleMiShuObj.RoleID);
            RoleMiShu roleMiShu = Singleton<NHManager>.Instance.CriteriaGet<RoleMiShu>(nHCriteriamishu);
            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>收到获取秘术的数组" + roleMiShu.MiShuIDArray);
            Utility.Assert.NotNull(roleMiShu.MiShuIDArray, () =>
            {

                var rMiShuObj = Singleton<NHManager>.Instance.CriteriaGet<RoleMiShu>(nHCriteriamishu);
                string rolemishuJson = rMiShuObj.MiShuIDArray;
                List<string> roleIDlist;
                List<MiShu> miShuIdList = new List<MiShu>();
                List<NHCriteria> nHCriteriaslist = new List<NHCriteria>();
                Utility.Assert.NotNull(rolemishuJson, () =>
                {
                    roleIDlist = new List<string>();
                    roleIDlist = Utility.Json.ToObject<List<string>>(rolemishuJson);
                    for (int i = 0; i < roleIDlist.Count; i++)
                    {
                        NHCriteria tmpcriteria = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", int.Parse(roleIDlist[i]));
                        MiShu miShu = Singleton<NHManager>.Instance.CriteriaGet<MiShu>(tmpcriteria);
                        miShuIdList.Add(miShu);
                        nHCriteriaslist.Add(tmpcriteria);

                    }
                }); SetResponseData(() =>
                {
                    SubDict.Add((byte)ObjectParameterCode.MiShu, Utility.Json.ToJson(miShuIdList));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
                Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaslist);
            }, () => SetResponseData(() =>
            {
                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>收到获取秘术的id为空");
                SubDict.Add((byte)ObjectParameterCode.MiShu, Utility.Json.ToJson(new List<string>()));
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
            }));
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriamishu);
        }
    }
}