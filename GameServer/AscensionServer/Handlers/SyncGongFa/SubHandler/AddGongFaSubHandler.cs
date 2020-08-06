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
            AscensionServer._Log.Info("添加的新的功法为");
            string gfJson = Convert.ToString(Utility.GetValue(dict,(byte)ParameterCode.GongFa));
            string  roleJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Role));

            var roleObj = Utility.Json.ToObject<Role>(roleJson);
            var gongfaObj = Utility.Json.ToObject<CultivationMethod>(gfJson);


            NHCriteria nHCriteriaRoleID = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            var roleGongFaObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleGongFa>(nHCriteriaRoleID);
            Dictionary<int, int> gongfaDict;
            Dictionary<int, string> DOdict=new Dictionary<int, string>();
            if (roleGongFaObj!=null)
            {
                if (!string.IsNullOrEmpty(roleGongFaObj.GongFaIDArray))
                {
                    gongfaDict= Utility.Json.ToObject<Dictionary<int, int>>(roleGongFaObj.GongFaIDArray);
                    if (gongfaDict.Values.ToList().Contains(gongfaObj.CultivationMethodID))
                    {
                        AscensionServer._Log.Info("人物已经学会此功法无法添加新的功法");
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;

                        Owner.ResponseData.Add((byte)ParameterCode.RoleGongFa, null);
                    }
                    else
                    {
                        CultivationMethod cultivationMethod=new CultivationMethod() { CultivationMethodID = gongfaObj.CultivationMethodID };
                        cultivationMethod=ConcurrentSingleton<NHManager>.Instance.Insert(cultivationMethod);
                        gongfaDict.Add(cultivationMethod.ID, cultivationMethod.CultivationMethodID);
                        roleGongFaObj.GongFaIDArray = Utility.Json.ToJson(gongfaDict);
                        ConcurrentSingleton<NHManager>.Instance.Update(roleGongFaObj);
                        DOdict.Add(0,Utility.Json.ToJson(cultivationMethod));
                        DOdict.Add(1, Utility.Json.ToJson(roleGongFaObj));


                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                        Owner.ResponseData.Add((byte)ParameterCode.RoleGongFa, Utility.Json.ToJson(DOdict));
                    }
                }
            }
            #region
            //if (roleGongFaObj != null)
            //{
            //    AscensionServer._Log.Info("添加的新的功法为2" + gfJson);
            //    if (!string.IsNullOrEmpty(roleGongFaObj.GongFaIDArray))
            //    {
            //        gongfaDict = new Dictionary<int, int>();
            //        gongfaDict = Utility.Json.ToObject<Dictionary<int, int>>(roleGongFaObj.GongFaIDArray);
            //        if (gongfaDict.Count >= 12)
            //        {
            //            SetResponseData(() =>
            //            {
            //                SubDict.Add((byte)ParameterCode.GongFa, Utility.Json.ToJson(new List<string>()));
            //                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            //                return;
            //            });
            //        }
            //        else
            //        {
            //            gongfaObj = ConcurrentSingleton<NHManager>.Instance.Insert(gongfaObj);
            //            gongfaDict.Add(gongfaObj.ID, gongfaObj.CultivationMethodID);

            //            ConcurrentSingleton<NHManager>.Instance.Update(new RoleGongFa() { RoleID = roleObj.RoleID, GongFaIDArray = Utility.Json.ToJson(gongfaDict) });

            //            DOdict.Add(1, Utility.Json.ToJson(gongfaObj));
            //            Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            //        }
            //    }
            //    else
            //    {
            //        gongfaDict = new Dictionary<int, int>();

            //        gongfaObj = ConcurrentSingleton<NHManager>.Instance.Insert<CultivationMethod>(gongfaObj);
            //        gongfaDict.Add(gongfaObj.ID, gongfaObj.CultivationMethodID);

            //        DOdict.Add(1, Utility.Json.ToJson(gongfaObj));
            //        ConcurrentSingleton<NHManager>.Instance.Update(new RoleGongFa() { RoleID = roleObj.RoleID, GongFaIDArray = Utility.Json.ToJson(gongfaDict) });
            //        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            //    }
            //}
            //else
            //{
            //    roleGongFaObj = ConcurrentSingleton<NHManager>.Instance.Insert(new RoleGongFa() { RoleID = roleObj.RoleID });
            //    gongfaDict = new Dictionary<int, int>();

            //    gongfaObj = ConcurrentSingleton<NHManager>.Instance.Insert<CultivationMethod>(gongfaObj);
            //    gongfaDict.Add(gongfaObj.ID, gongfaObj.CultivationMethodID);

            //    DOdict.Add(1, Utility.Json.ToJson(gongfaObj));
            //    ConcurrentSingleton<NHManager>.Instance.Update(new RoleGongFa() { RoleID = roleObj.RoleID, GongFaIDArray = Utility.Json.ToJson(gongfaDict) });

            //    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            //}
            //var roleGongFaSendObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleGongFa>(nHCriteriaRoleID);
            //DOdict.Add(2, Utility.Json.ToJson(roleGongFaSendObj) );
            #endregion
            Owner.OpResponse.Parameters = Owner.ResponseData;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID);
        }
        
    }
}
