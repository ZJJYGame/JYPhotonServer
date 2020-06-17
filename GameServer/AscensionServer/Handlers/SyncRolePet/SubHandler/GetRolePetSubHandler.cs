using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    public class GetRolePetSubHandler : SyncRolePetSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {

            var dict = ParseSubDict(operationRequest);
            string rolepet = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.RolePet));

            var rolepetObj = Utility.Json.ToObject<RolePet>(rolepet);
            NHCriteria nHCriteriaRolePet = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolepetObj.RoleID);

            var rolepets = Singleton<NHManager>.Instance.CriteriaSelect<RolePet>(nHCriteriaRolePet);
            if (rolepets != null)
            {
                AscensionServer._Log.Info(">>>>>>>>>>>>>》》》》》》》》》》》》>>获得宠物进来了");
                var rpetObj = Singleton<NHManager>.Instance.CriteriaSelect<RolePet>(nHCriteriaRolePet);
                string RolePetList = rpetObj.PetIDDict;
                Dictionary<int, int> petIDList;
                List<Pet> petlist = new List<Pet>();
                List<NHCriteria> nHCriteriasList = new List<NHCriteria>();
                if (!string.IsNullOrEmpty(RolePetList))
                {
                    AscensionServer._Log.Info(">>>>>>>>>>>>>》》》》》》》》》》》》>>获得宠物进来了"+ RolePetList);
                    petIDList = new Dictionary<int, int>();
                    petIDList = Utility.Json.ToObject<Dictionary<int, int>>(RolePetList);
                    foreach (var petid in petIDList)
                    {
                        NHCriteria nHCriteriapet = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", petid.Key);
                        Pet petObj = Singleton<NHManager>.Instance.CriteriaSelect<Pet>(nHCriteriapet);
                        petlist.Add(petObj);
                        nHCriteriasList.Add(nHCriteriapet);
                    }
                }
                SetResponseData(() =>
                         {
                             SubDict.Add((byte)ObjectParameterCode.RolePet, Utility.Json.ToJson(petlist));
                             Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                         });
                Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriasList);
            }
            else
            {
                SetResponseData(() =>
                {
                    AscensionServer._Log.Info(">>>>>>>>>>>>>》》》》》》》》》》》》>>获得宠物失败");
                    SubDict.Add((byte)ObjectParameterCode.RolePet, Utility.Json.ToJson(new List<string>()));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRolePet);
        }
    }
}
