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
            string rmsJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.RoleMiShu));


            var rolemishuObj = Utility.ToObject<RoleMiShu>(rmsJson);
            var mishuObj = Utility.ToObject<MiShu>(msJson);
            #region BugFix
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolemishuObj.RoleID);
            var roleMiShuObj= Singleton<NHManager>.Instance.CriteriaGet<RoleMiShu>(nHCriteriaRoleID);
            if (roleMiShuObj==null)
            {
                Singleton<NHManager>.Instance.Add(rolemishuObj);
                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler  add roleMiShuObj" + rmsJson+ "\n >>>>>>>>>>>>");
                mishuObj = Singleton<NHManager>.Instance.Add(mishuObj);
                List<string> miShuIDList = new List<string>();
                miShuIDList.Add(mishuObj.ID.ToString());
                rolemishuObj.MiShuIDArray = Utility.ToJson(miShuIDList);
            }
            else
            {
                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler  Update roleMiShuObj" + rmsJson + "\n >>>>>>>>>>>>");
                mishuObj = Singleton<NHManager>.Instance.Add(mishuObj);
                List<string> miShuIDList = new List<string>();
                if (string.IsNullOrEmpty(roleMiShuObj.MiShuIDArray))
                {
                    AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler  Update  empty roleMiShuObj" + rmsJson + "\n >>>>>>>>>>>>");

                    miShuIDList.Add(mishuObj.ID.ToString());
                }
                else
                {
                    AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler  Update existed roleMiShuObj" + rmsJson + "\n >>>>>>>>>>>>");

                    miShuIDList = Utility.ToObject<List<string>>(roleMiShuObj.MiShuIDArray);
                    if(!miShuIDList.Contains(mishuObj.ID.ToString()))
                        miShuIDList.Add(mishuObj.ID.ToString());
                    else
                        AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler mishu is already existed !!" + rmsJson + "\n >>>>>>>>>>>>");
                }
                rolemishuObj.MiShuIDArray = Utility.ToJson(miShuIDList);
            }
            Singleton<NHManager>.Instance.Update<RoleMiShu>(rolemishuObj);

            //var arrayResult = Utility.ToObject<List<string>>(roleMiShuObj.MiShuIDArray);
            //AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler"+arrayResult+"\n >>>>>>>>>>>>");
            //mishuObj = Singleton<NHManager>.Instance.Add(mishuObj);
            //arrayResult.Add(mishuObj.ID.ToString());
            //AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler" + "success"+ "\n >>>>>>>>>>>>");

            #endregion

            #region YZQ
            //NHCriteria nHCriteriaMiShu = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("MiShuID", mishuObj.MiShuID);
            //NHCriteria nHCriteriamishuid = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", mishuObj.ID);

            //NHCriteria nHCriteriaRoleMiShu = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolemishuObj.RoleID);

            //var RoleMiShuObj = Singleton<NHManager>.Instance.CriteriaGet<RoleMiShu>(nHCriteriaRoleMiShu);//查询当前表中是否存在
            //var mishu = Singleton<NHManager>.Instance.CriteriaGet<MiShu>(nHCriteriamishuid);

            //Utility.Assert.IsNull(mishu, () =>
            //{
            //    Singleton<NHManager>.Instance.Add(mishuObj);
            //    var mishuID = Singleton<NHManager>.Instance.CriteriaGet<MiShu>(nHCriteriamishuid);

            //    AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> 判断存在秘术进来了 >>>>>>>>>>>>" );
            //    Utility.Assert.NotNull(RoleMiShuObj, () =>
            //    {
            //        string MSJson = RoleMiShuObj.MiShuIDArray;
            //        AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> 更新角色秘書进来了 >>>>>>>>>>>>" + MSJson);
            //        List<string> RoleMiShuList = new List<string>();
            //        if (!string.IsNullOrEmpty(MSJson))
            //        {
            //            RoleMiShuList = Utility.ToObject<List<string>>(MSJson);
            //            string msid = mishuID.ID.ToString();
            //            if (!string.IsNullOrEmpty(MSJson))
            //            {
            //                RoleMiShuList.Add(msid);
            //            }
            //            else
            //                RoleMiShuList.Add(msid);
            //            Singleton<NHManager>.Instance.Update(new RoleMiShu() { RoleID = rolemishuObj.RoleID, MiShuIDArray = Utility.ToJson(RoleMiShuList) });
            //            Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            //        }
            //    }, () => {
            //        Singleton<NHManager>.Instance.Add(new RoleMiShu() { RoleID = rolemishuObj.RoleID });

            //        string MSJson = RoleMiShuObj.MiShuIDArray;
            //        AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> 添加角色秘書进来了 >>>>>>>>>>>>" + MSJson);
            //        List<string> RoleMiShuList = new List<string>();
            //        if (string.IsNullOrEmpty(MSJson))
            //        {
            //            RoleMiShuList = Utility.ToObject<List<string>>(MSJson);
            //            string msid = mishuID.ID.ToString();
            //            if (string.IsNullOrEmpty(MSJson))
            //            {
            //                RoleMiShuList.Add(msid);
            //            }
            //            else
            //                RoleMiShuList.Add(msid);
            //            Singleton<NHManager>.Instance.Update(new RoleMiShu() { RoleID = rolemishuObj.RoleID, MiShuIDArray = Utility.ToJson(RoleMiShuList) });
            //            Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            //        }
            //    }
            //    );
            //}, () => Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail);

            //peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            //Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleMiShu, nHCriteriaMiShu);
            #endregion
        }
    }
}
