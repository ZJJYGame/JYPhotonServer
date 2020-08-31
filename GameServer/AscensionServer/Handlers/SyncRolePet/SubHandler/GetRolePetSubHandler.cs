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
            string rolepet = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RolePet));

            var rolepetObj = Utility.Json.ToObject<RolePet>(rolepet);
            NHCriteria nHCriteriaRolePet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", rolepetObj.RoleID);

            var rolepets = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RolePet>(nHCriteriaRolePet);
            if (rolepets != null)
            {
                var rpetObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RolePet>(nHCriteriaRolePet);
                string RolePetList = rpetObj.PetIDDict;
                Dictionary<int, int> petIDList;
                List<Pet> petlist = new List<Pet>();
                List<NHCriteria> nHCriteriasList = new List<NHCriteria>();
                if (!string.IsNullOrEmpty(RolePetList))
                {
                    petIDList = new Dictionary<int, int>();
                    petIDList = Utility.Json.ToObject<Dictionary<int, int>>(RolePetList);
                    foreach (var petid in petIDList)
                    {
                        NHCriteria nHCriteriapet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", petid.Key);
                        Pet petObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Pet>(nHCriteriapet);
                        petlist.Add(petObj);
                        nHCriteriasList.Add(nHCriteriapet);
                    }
                }
                SetResponseData(() =>
                         {
                             SubDict.Add((byte)ParameterCode.RolePet, Utility.Json.ToJson(petlist));
                             Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                         });
                GameManager.ReferencePoolManager.Despawns(nHCriteriasList);
            }
            else
            {
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.RolePet, Utility.Json.ToJson(new List<string>()));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaRolePet);
        }
    }
}
