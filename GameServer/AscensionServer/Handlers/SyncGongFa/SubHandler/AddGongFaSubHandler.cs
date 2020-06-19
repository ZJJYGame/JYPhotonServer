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
            AscensionServer._Log.Info("添加功法ID》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》" );
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolegongfaObj.RoleID);
            var roleGongFaObj = Singleton<NHManager>.Instance.CriteriaSelect<RoleGongFa>(nHCriteriaRoleID);
            Dictionary<int, int> gongfaDict;
            Dictionary<int, DataObject> DOdict=new Dictionary<int, DataObject>();
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
                        
                        Singleton<NHManager>.Instance.Update(new RoleGongFa() { RoleID = rolegongfaObj.RoleID, GongFaIDArray = Utility.Json.ToJson(gongfaDict) });
                        AscensionServer._Log.Info("添加1功法ID》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》" + Utility.Json.ToJson(gongfaDict));
                        DOdict.Add(1, gongfaObj);
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    }
                }
                else
                {
                    gongfaDict = new Dictionary<int, int>();
                    //gongfaDict = Utility.Json.ToObject<Dictionary<int, int>>(roleGongFaObj.GongFaIDArray);
                    gongfaObj = Singleton<NHManager>.Instance.Insert<GongFa>(gongfaObj);
                    gongfaDict.Add(gongfaObj.ID, gongfaObj.GongFaID);
                   AscensionServer._Log.Info("添加2功法ID》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》" + Utility.Json.ToJson(gongfaDict));
                    DOdict.Add(1, gongfaObj);
                    Singleton<NHManager>.Instance.Update(new RoleGongFa() { RoleID = rolegongfaObj.RoleID, GongFaIDArray = Utility.Json.ToJson(gongfaDict) });
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
            }
            else
            {
                rolegongfaObj= Singleton<NHManager>.Instance.Insert(new RoleGongFa() { RoleID = rolegongfaObj.RoleID });
                gongfaDict = new Dictionary<int, int>();
               // gongfaDict = Utility.Json.ToObject<Dictionary<int, int>>(roleGongFaObj.GongFaIDArray);
                gongfaObj = Singleton<NHManager>.Instance.Insert<GongFa>(gongfaObj);
                gongfaDict.Add(gongfaObj.ID, gongfaObj.GongFaID);
                AscensionServer._Log.Info("添加3功法ID》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》" + Utility.Json.ToJson(gongfaDict));
                DOdict.Add(1, gongfaObj);
                Singleton<NHManager>.Instance.Update(new RoleGongFa() { RoleID = rolegongfaObj.RoleID, GongFaIDArray = Utility.Json.ToJson(gongfaDict) });
                
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            var roleGongFaSendObj = Singleton<NHManager>.Instance.CriteriaSelect<RoleGongFa>(nHCriteriaRoleID);
            DOdict.Add(2, roleGongFaSendObj);
            Owner.OpResponse.Parameters = Owner.ResponseData;
            AscensionServer._Log.Info("发送功法ID》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》" + Utility.Json.ToJson(DOdict));
            Owner.ResponseData.Add((byte)ObjectParameterCode.RoleGongFa, Utility.Json.ToJson(DOdict));
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID);
        }
        
    }
}
