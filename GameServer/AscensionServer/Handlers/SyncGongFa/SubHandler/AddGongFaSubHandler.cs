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
            var gongfaObj = Utility.Json.ToObject<CultivationMethod>(gfJson);

           
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolegongfaObj.RoleID);
            var roleGongFaObj = Singleton<NHManager>.Instance.CriteriaSelect<RoleGongFa>(nHCriteriaRoleID);
            Dictionary<int, int> gongfaDict;
            Dictionary<int, string> DOdict=new Dictionary<int, string>();
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
                        gongfaDict.Add(gongfaObj.ID, gongfaObj.CultivationMethodID);
                        
                        Singleton<NHManager>.Instance.Update(new RoleGongFa() { RoleID = rolegongfaObj.RoleID, GongFaIDArray = Utility.Json.ToJson(gongfaDict) });

                        DOdict.Add(1, Utility.Json.ToJson(gongfaObj));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    }
                }
                else
                {
                    gongfaDict = new Dictionary<int, int>();

                    gongfaObj = Singleton<NHManager>.Instance.Insert<CultivationMethod>(gongfaObj);
                    gongfaDict.Add(gongfaObj.ID, gongfaObj.CultivationMethodID);
  
                    DOdict.Add(1, Utility.Json.ToJson(gongfaObj));
                    Singleton<NHManager>.Instance.Update(new RoleGongFa() { RoleID = rolegongfaObj.RoleID, GongFaIDArray = Utility.Json.ToJson(gongfaDict) });
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
            }
            else
            {
                rolegongfaObj= Singleton<NHManager>.Instance.Insert(new RoleGongFa() { RoleID = rolegongfaObj.RoleID });
                gongfaDict = new Dictionary<int, int>();

                gongfaObj = Singleton<NHManager>.Instance.Insert<CultivationMethod>(gongfaObj);
                gongfaDict.Add(gongfaObj.ID, gongfaObj.CultivationMethodID);

                DOdict.Add(1, Utility.Json.ToJson(gongfaObj));
                Singleton<NHManager>.Instance.Update(new RoleGongFa() { RoleID = rolegongfaObj.RoleID, GongFaIDArray = Utility.Json.ToJson(gongfaDict) });
                
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            var roleGongFaSendObj = Singleton<NHManager>.Instance.CriteriaSelect<RoleGongFa>(nHCriteriaRoleID);
            DOdict.Add(2, Utility.Json.ToJson(roleGongFaSendObj) );
            Owner.OpResponse.Parameters = Owner.ResponseData;

            Owner.ResponseData.Add((byte)ParameterCode.RoleGongFa, Utility.Json.ToJson(DOdict));
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID);
        }
        
    }
}
