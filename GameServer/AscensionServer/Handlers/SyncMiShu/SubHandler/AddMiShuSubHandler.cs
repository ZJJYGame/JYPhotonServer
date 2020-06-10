using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
namespace AscensionServer
{
    public class AddMiShuSubHandler : SyncMiShuSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);

            string msJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.MiShu));
            string rmsJson = Convert.ToString(Utility.GetValue(dict, (byte)200));


            var rolemishuObj = Utility.ToObject<RoleMiShu>(rmsJson);
            var mishuObj = Utility.ToObject<MiShu>(msJson);

            //AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> 添加 JSON 秘术进来了 >>>>>>>>>>>>" + msJson);
            NHCriteria nHCriteriaMiShu = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("MiShuID", mishuObj.MiShuID);

            NHCriteria nHCriteriaRoleMiShu = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolemishuObj.RoleID);
       
            var RoleMiShuObj = Singleton<NHManager>.Instance.CriteriaGet<RoleMiShu>(nHCriteriaRoleMiShu);//查询当前表中是否存在
            var mishu = Singleton<NHManager>.Instance.CriteriaGet<MiShu>(nHCriteriaMiShu);

            Utility.Assert.IsNull(mishu, () =>
            {
                Singleton<NHManager>.Instance.Add(mishuObj);
                NHCriteria nHCriteriamishuid = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", mishuObj.ID);
                var mishuID = Singleton<NHManager>.Instance.CriteriaGet<MiShu>(nHCriteriamishuid);
                // AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> 添加 JSON 秘术进来了 >>>>>>>>>>>>" + msJson);
                string MSJson = RoleMiShuObj.MiShuIDArray;
                Utility.Assert.NotNull(RoleMiShuObj, () =>
                 {

                     List<string> RoleMiShuList = new List<string>();
                     if (!string.IsNullOrEmpty(MSJson))
                     {
                         RoleMiShuList = Utility.ToObject<List<string>>(MSJson);
                         string msid = mishuID.ID.ToString();
                         if (!string.IsNullOrEmpty(MSJson))
                         {
                             RoleMiShuList.Add(msid);
                         }
                         else
                             RoleMiShuList.Add(msid);
                         Singleton<NHManager>.Instance.Update(new RoleMiShu() { RoleID = rolemishuObj.RoleID, MiShuIDArray = Utility.ToJson(RoleMiShuList) });
                         Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                     }
                 }, () => {
                     AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> 添加角色秘書进来了 >>>>>>>>>>>>");
                     List<string> RoleMiShuList = new List<string>();
                     if (!string.IsNullOrEmpty(MSJson))
                     {
                         RoleMiShuList = Utility.ToObject<List<string>>(MSJson);
                         string msid = mishuID.ID.ToString();
                         if (!string.IsNullOrEmpty(MSJson))
                         {
                             RoleMiShuList.Add(msid);
                         }
                         else
                             RoleMiShuList.Add(msid);
                         Singleton<NHManager>.Instance.Add(new RoleMiShu() { RoleID = rolemishuObj.RoleID, MiShuIDArray = Utility.ToJson(RoleMiShuList) });
                         Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                     }
                 }
                );
            }, () => Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail);

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleMiShu, nHCriteriaMiShu);

        }
    }
}
