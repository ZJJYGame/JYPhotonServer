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
    public class AddGongFaSubHandler : SyncGongFaSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);

            string gfJson = Convert.ToString(Utility.GetValue(dict,(byte)ParameterCode.GongFa));
            string rgfJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleGongFa));

            var rolegongfaObj = Utility.Json.ToObject<RoleGongFa>(rgfJson);
            var gongfaObj = Utility.Json.ToObject<GongFa>(gfJson);

            #region OldFunction
            //if (roleGongFaObj == null)
            //{
            //    Singleton<NHManager>.Instance.Insert(rolegongfaObj);
            //    AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler  add roleMiShuObj" + rgfJson + "\n >>>>>>>>>>>>");
            //    gongfaObj = Singleton<NHManager>.Instance.Insert(gongfaObj);
            //    List<string> gongfaIDList = new List<string>();
            //    gongfaIDList.Add(gongfaObj.ID.ToString());
            //    rolegongfaObj.GongFaIDArray = Utility.Json.ToJson(gongfaIDList);
            //}
            //else
            //{
            //    AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler  Update roleMiShuObj" + rgfJson + "\n >>>>>>>>>>>>");
            //    gongfaObj = Singleton<NHManager>.Instance.Insert(gongfaObj);
            //    List<string> gongfaIDList = new List<string>();
            //    if (string.IsNullOrEmpty(roleGongFaObj.GongFaIDArray))
            //    {
            //        AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler  Update  empty roleMiShuObj" + rgfJson + "\n >>>>>>>>>>>>");

            //        gongfaIDList.Add(gongfaObj.ID.ToString());
            //    }
            //    else
            //    {
            //        AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler  Update existed roleMiShuObj" + rgfJson + "\n >>>>>>>>>>>>");

            //        gongfaIDList = Utility.Json.ToObject<List<string>>(roleGongFaObj.GongFaIDArray);
            //        if (!gongfaIDList.Contains(gongfaObj.ID.ToString()))
            //        {
            //            gongfaIDList.Add(gongfaObj.ID.ToString());
            //        }

            //        else
            //            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler mishu is already existed !!" + rgfJson + "\n >>>>>>>>>>>>");
            //    }
            //    rolegongfaObj.GongFaIDArray = Utility.Json.ToJson(gongfaIDList);
            //    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            //}
            #endregion

            AscensionServer._Log.Info("添加秘术》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》");
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolegongfaObj.RoleID);
            var roleGongFaObj = Singleton<NHManager>.Instance.CriteriaSelect<RoleGongFa>(nHCriteriaRoleID);
            Dictionary<int, int> gongfaDict;
            if (roleGongFaObj != null)
            {
                if (!string.IsNullOrEmpty(roleGongFaObj.GongFaIDArray))
                {
                    gongfaDict = new Dictionary<int, int>();
                    gongfaDict = Utility.Json.ToObject<Dictionary<int, int>>(roleGongFaObj.GongFaIDArray);
                    if (gongfaDict.Count >= 12)
                    {
                        SetResponseData(() =>
                        {
                            SubDict.Add((byte)ParameterCode.OnOffLine, Utility.Json.ToJson(new List<string>()));
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                            return;
                        });
                    }
                    else
                    {
                        gongfaObj = Singleton<NHManager>.Instance.Insert(gongfaObj);
                        gongfaDict.Add(gongfaObj.ID, gongfaObj.GongFaID);
                        //AscensionServer._Log.Info("添加1秘术ID》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》"+ mishuObj.ID);
                        Singleton<NHManager>.Instance.Update(new RoleMiShu() { RoleID = rolegongfaObj.RoleID, MiShuIDArray = Utility.Json.ToJson(gongfaDict) });
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    }
                }
                else
                {
                    gongfaDict = new Dictionary<int, int>();
                    gongfaObj = Singleton<NHManager>.Instance.Insert(gongfaObj);
                    gongfaDict.Add(gongfaObj.ID, gongfaObj.GongFaID);
                    //AscensionServer._Log.Info("添加2秘术ID》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》" + mishuObj.ID);
                    Singleton<NHManager>.Instance.Update(new RoleMiShu() { RoleID = rolegongfaObj.RoleID, MiShuIDArray = Utility.Json.ToJson(gongfaDict) });
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
            }
            else
            {
                gongfaDict = new Dictionary<int, int>();
                gongfaObj = Singleton<NHManager>.Instance.Insert(gongfaObj);
                gongfaDict.Add(gongfaObj.ID, gongfaObj.GongFaID);
                //AscensionServer._Log.Info("添加3秘术ID》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》" + Utility.Json.ToJson(mishuDict));
                Singleton<NHManager>.Instance.Update(new RoleMiShu() { RoleID = rolegongfaObj.RoleID, MiShuIDArray = Utility.Json.ToJson(gongfaDict) });

                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            Singleton<NHManager>.Instance.Update<RoleGongFa>(rolegongfaObj);
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
        
    }
}
