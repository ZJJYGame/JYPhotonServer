using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
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


            var rolemishuObj = Utility.Json.ToObject<RoleMiShu>(rmsJson);
            var mishuObj = Utility.Json.ToObject<MiShu>(msJson);
            #region BugFix
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolemishuObj.RoleID);

            var roleMiShuObj = Singleton<NHManager>.Instance.CriteriaGet<RoleMiShu>(nHCriteriaRoleID);
            if (roleMiShuObj == null)
            {
                Singleton<NHManager>.Instance.Add(rolemishuObj);
                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler  add roleMiShuObj" + rmsJson + "\n >>>>>>>>>>>>");
                mishuObj = Singleton<NHManager>.Instance.Add(mishuObj);

                List<string> miShuIDList = new List<string>();
                miShuIDList.Add(mishuObj.ID.ToString());
                rolemishuObj.MiShuIDArray = Utility.Json.ToJson(miShuIDList);
            }
            else
            {
                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler  Update roleMiShuObj" + rmsJson + "\n >>>>>>>>>>>>");
                mishuObj = Singleton<NHManager>.Instance.Insert(mishuObj);
                List<string> miShuIDList = new List<string>();
                if (string.IsNullOrEmpty(roleMiShuObj.MiShuIDArray))
                {
                    AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler  Update  empty roleMiShuObj" + rmsJson + "\n >>>>>>>>>>>>");

                    miShuIDList.Add(mishuObj.ID.ToString());
                }
                else
                {
                    AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler  Update existed roleMiShuObj" + rmsJson + "\n >>>>>>>>>>>>");

                    miShuIDList = Utility.Json.ToObject<List<string>>(roleMiShuObj.MiShuIDArray);
                    if (!miShuIDList.Contains(mishuObj.ID.ToString()))
                    {
                        miShuIDList.Add(mishuObj.ID.ToString());
                    }

                    else
                        AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler mishu is already existed !!" + rmsJson + "\n >>>>>>>>>>>>");
                }
                rolemishuObj.MiShuIDArray = Utility.Json.ToJson(miShuIDList);
            }
            Singleton<NHManager>.Instance.Update<RoleMiShu>(rolemishuObj);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID);
            #endregion


            #region YZQ

            //NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolemishuObj.RoleID);
            //var roleMiShuObj = Singleton<NHManager>.Instance.CriteriaGet<RoleMiShu>(nHCriteriaRoleID);
            //Dictionary<int, int> mishuDict;
            //Utility.Assert.NotNull(roleMiShuObj, () =>
            //{
            //    if (!string.IsNullOrEmpty(roleMiShuObj.MiShuIDArray))
            //    {
            //        mishuDict = new Dictionary<int, int>();
            //        mishuDict = Utility.Json.ToObject<Dictionary<int, int>>(roleMiShuObj.MiShuIDArray);
            //        if (mishuDict.Count >= 12)
            //        {
            //            SetResponseData(() =>
            //            {
            //                SubDict.Add((byte)ObjectParameterCode.OnOffLine, Utility.Json.ToJson(new List<string>()));
            //                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            //                return;
            //            });
            //        }
            //        else
            //        {
            //            mishuObj = Singleton<NHManager>.Instance.Add(mishuObj);
            //            mishuDict.Add(mishuObj.ID, mishuObj.MiShuID);
            //            Singleton<NHManager>.Instance.Update(new RoleMiShuDTO() { RoleID = rolemishuObj.RoleID, MiShuIDArray = Utility.Json.ToJson(mishuDict) });
            //            Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            //        }
            //    }
            //    else
            //    {
            //        mishuDict = new Dictionary<int, int>();
            //        mishuObj = Singleton<NHManager>.Instance.Add(mishuObj);
            //        mishuDict.Add(mishuObj.ID, mishuObj.MiShuID);
            //        Singleton<NHManager>.Instance.Update(new RoleMiShuDTO() { RoleID = rolemishuObj.RoleID, MiShuIDArray = Utility.Json.ToJson(mishuDict) });
            //        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            //    }
            //},()=>
            //{
            //    mishuDict = new Dictionary<int, int>();
            //    mishuObj = Singleton<NHManager>.Instance.Add(mishuObj);
            //    mishuDict.Add(mishuObj.ID, mishuObj.MiShuID);
            //    Singleton<NHManager>.Instance.Update(new RoleMiShuDTO() { RoleID = rolemishuObj.RoleID, MiShuIDArray = Utility.Json.ToJson(mishuDict) });
            //    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            //});
            //peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            //Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID);
            #endregion
        }

    }
}
