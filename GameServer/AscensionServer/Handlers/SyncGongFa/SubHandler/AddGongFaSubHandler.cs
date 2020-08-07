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
            string  roleJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Role));

            var roleObj = Utility.Json.ToObject<Role>(roleJson);
            var gongfaObj = Utility.Json.ToObject<CultivationMethod>(gfJson);
            AscensionServer._Log.Info("添加的新的功法为"+ roleJson);

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
                        AscensionServer._Log.Info("人物已经学会的功法" + roleGongFaObj.GongFaIDArray);
                        AscensionServer._Log.Info("人物已经学会此功法无法添加新的功法"+ gongfaObj.CultivationMethodID);
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

                        SetResponseData(() =>
                        {
                            SubDict.Add((byte)ParameterCode.RoleGongFa, Utility.Json.ToJson(DOdict));
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                        });

                    }
                }
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID);
        }
        
    }
}
