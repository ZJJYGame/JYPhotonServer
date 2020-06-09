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
    public class AddMiShuSubHandler : SyncMiShuSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>添加秘术进来了 >>>>>>>>>>>>");
            var dict = ParseSubDict(operationRequest);
            string roleMSJson = Convert.ToString(Utility.GetValue(dict,(byte)ObjectParameterCode.MiShu));
            ///
            var mishuObj = Utility.ToObject<MiShu>(roleMSJson);
            NHCriteria nHCriteriaMiShu = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("MiShuID", mishuObj.MiShuID);
            ///
            var roleMiShuObj = Utility.ToObject<RoleMiShu>(roleMSJson);
            NHCriteria nHCriteriaRoleMiShu = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID",roleMiShuObj.RoleID);

            var RoleMiShuObj = Singleton<NHManager>.Instance.CriteriaGet<RoleMiShu>(nHCriteriaRoleMiShu);//查询当前表中是否存在
          
            //mishuObj = Singleton<NHManager>.Instance.Add(mishuObj);
            //NHCriteria nHCriteriaID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", mishuObj.ID);
            //var mishuidObj = Singleton<NHManager>.Instance.CriteriaGet<MiShu>(nHCriteriaID);//添加秘术
            //if (RoleMiShuObj!=null)
            //{
               

            //    string rolemishuJson = RoleMiShuObj.MiShuIDArray;
            //    List<string> RoleMiShuList = new List<string>();
            //    if (!string.IsNullOrEmpty(rolemishuJson))
            //    {
            //        RoleMiShuList = Utility.ToObject<List<string>>(rolemishuJson);

            //    }
            //}


        }
    }
}
