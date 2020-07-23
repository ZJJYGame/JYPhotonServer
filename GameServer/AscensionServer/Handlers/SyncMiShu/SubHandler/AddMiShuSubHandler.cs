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

            string msJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.MiShu));
            string rmsJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleMiShu));


            var rolemishuObj = Utility.Json.ToObject<RoleMiShu>(rmsJson);
            var mishuObj = Utility.Json.ToObject<MiShu>(msJson);



            NHCriteria nHCriteriaRoleID = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolemishuObj.RoleID);
            var roleMiShuObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleMiShu>(nHCriteriaRoleID);
            Dictionary<int, int> mishuDict;
            Dictionary<int, string> DOdict = new Dictionary<int, string>();
            if (roleMiShuObj != null)
            {
                if (!string.IsNullOrEmpty(roleMiShuObj.MiShuIDArray))
                {
                    mishuDict = new Dictionary<int, int>();
                    mishuDict = Utility.Json.ToObject<Dictionary<int, int>>(roleMiShuObj.MiShuIDArray);
                    if (mishuDict.Count >= 12)
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
                        mishuObj = ConcurrentSingleton<NHManager>.Instance.Insert(mishuObj);
                        mishuDict.Add(mishuObj.ID, mishuObj.MiShuID);

                        ConcurrentSingleton<NHManager>.Instance.Update(new RoleMiShu() { RoleID = rolemishuObj.RoleID, MiShuIDArray = Utility.Json.ToJson(mishuDict) });
                        DOdict.Add(1,Utility.Json.ToJson(mishuObj));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    }
                }
                else
                {
                    mishuDict = new Dictionary<int, int>();
                    mishuDict = Utility.Json.ToObject<Dictionary<int, int>>(roleMiShuObj.MiShuIDArray);
                    mishuObj = ConcurrentSingleton<NHManager>.Instance.Insert(mishuObj);
                    mishuDict.Add(mishuObj.ID, mishuObj.MiShuID);

                    ConcurrentSingleton<NHManager>.Instance.Update(new RoleMiShu() { RoleID = rolemishuObj.RoleID, MiShuIDArray = Utility.Json.ToJson(mishuDict) });
                    DOdict.Add(1, Utility.Json.ToJson(mishuObj));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
            }
            else
            {
                mishuDict = new Dictionary<int, int>();
                mishuDict = Utility.Json.ToObject<Dictionary<int, int>>(roleMiShuObj.MiShuIDArray);
                mishuObj = ConcurrentSingleton<NHManager>.Instance.Insert(mishuObj);
                mishuDict.Add(mishuObj.ID, mishuObj.MiShuID);

                ConcurrentSingleton<NHManager>.Instance.Update(new RoleMiShu() { RoleID = rolemishuObj.RoleID, MiShuIDArray = Utility.Json.ToJson(mishuDict) });
                DOdict.Add(1, Utility.Json.ToJson(mishuObj));
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            var roleMiShuSendObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleMiShu>(nHCriteriaRoleID);
            DOdict.Add(2,Utility.Json.ToJson(roleMiShuSendObj));
            Owner.OpResponse.Parameters = Owner.ResponseData;
            Owner.ResponseData.Add((byte)ParameterCode.RoleMiShu,Utility.Json.ToJson(DOdict));
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID);

        }

    }
}
