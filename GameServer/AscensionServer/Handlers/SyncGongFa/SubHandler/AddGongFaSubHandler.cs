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

            string gfJson = Convert.ToString(Utility.GetValue(dict,(byte)ObjectParameterCode.GongFa));
            string rgfJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.RoleGongFa));

            var rolegongfaObj = Utility.Json.ToObject<RoleGongFa>(rgfJson);
            var gongfaObj = Utility.Json.ToObject<GongFa>(gfJson);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolegongfaObj.RoleID);
            var roleGongFaObj = Singleton<NHManager>.Instance.CriteriaGet<RoleGongFa>(nHCriteriaRoleID);
            if (roleGongFaObj == null)
            {
                Singleton<NHManager>.Instance.Add(rolegongfaObj);
                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler  add roleMiShuObj" + rgfJson + "\n >>>>>>>>>>>>");
                gongfaObj = Singleton<NHManager>.Instance.Add(gongfaObj);
                List<string> gongfaIDList = new List<string>();
                gongfaIDList.Add(gongfaObj.ID.ToString());
                rolegongfaObj.GongFaIDArray = Utility.Json.ToJson(gongfaIDList);
            }
            else
            {
                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler  Update roleMiShuObj" + rgfJson + "\n >>>>>>>>>>>>");
                gongfaObj = Singleton<NHManager>.Instance.Add(gongfaObj);
                List<string> gongfaIDList = new List<string>();
                if (string.IsNullOrEmpty(roleGongFaObj.GongFaIDArray))
                {
                    AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler  Update  empty roleMiShuObj" + rgfJson + "\n >>>>>>>>>>>>");

                    gongfaIDList.Add(gongfaObj.ID.ToString());
                }
                else
                {
                    AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler  Update existed roleMiShuObj" + rgfJson + "\n >>>>>>>>>>>>");

                    gongfaIDList = Utility.Json.ToObject<List<string>>(roleGongFaObj.GongFaIDArray);
                    if (!gongfaIDList.Contains(gongfaObj.ID.ToString()))
                    {
                        gongfaIDList.Add(gongfaObj.ID.ToString());
                    }

                    else
                        AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>> \n AddMiShuSubHandler mishu is already existed !!" + rgfJson + "\n >>>>>>>>>>>>");
                }
                rolegongfaObj.GongFaIDArray = Utility.Json.ToJson(gongfaIDList);
                Owner.OpResponse.ReturnCode =(short) ReturnCode.Success;
            }
            peer.SendOperationResponse(Owner.OpResponse,sendParameters);
            Singleton<NHManager>.Instance.Update<RoleGongFa>(rolegongfaObj);
        }
        
    }
}
