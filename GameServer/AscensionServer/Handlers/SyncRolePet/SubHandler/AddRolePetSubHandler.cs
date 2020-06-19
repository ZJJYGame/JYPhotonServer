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
    public class AddRolePetSubHandler : SyncRolePetSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);

            string rpJson = Convert.ToString(Utility.GetValue(dict,(byte)ParameterCode.RolePet));
            string pJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Pet));
            string psJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.PetStatus));
            AscensionServer._Log.Info("添加宠物进来了》》》》》》》》》》》》》》》》》》》》》》》》》》》》》");
            var rolepetObj = Utility.Json.ToObject<RolePet>(rpJson);
            var petObj = Utility.Json.ToObject<Pet>(pJson);
            var petstatusObj = Utility.Json.ToObject<PetStatus>(psJson);
            NHCriteria nHCriteriaroleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolepetObj.RoleID);
            var rolepet = Singleton<NHManager>.Instance.CriteriaSelect<RolePet>(nHCriteriaroleID);
            Dictionary<int, int> petDict;
            if (rolepet!=null)
            {
                if (!string.IsNullOrEmpty(rolepet.PetIDDict))
                {
                    petDict = new Dictionary<int, int>();
                    petDict = Utility.Json.ToObject<Dictionary<int, int>>(rolepet.PetIDDict);

                    petObj = Singleton<NHManager>.Instance.Insert<Pet>(petObj);
                    petstatusObj.PetID = petObj.ID;
                    petstatusObj = Singleton<NHManager>.Instance.Insert<PetStatus>(petstatusObj);
                    petDict.Add(petObj.ID, petObj.PetID);
                    Singleton<NHManager>.Instance.Update(new RolePet() {RoleID = rolepet.RoleID,  PetIDDict = Utility.Json.ToJson(petDict) });
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
                else
                {
                    petDict = new Dictionary<int, int>();
                    petDict = Utility.Json.ToObject<Dictionary<int, int>>(rolepet.PetIDDict);

                    petObj = Singleton<NHManager>.Instance.Insert<Pet>(petObj);
                    petstatusObj.PetID = petObj.ID;
                    petstatusObj = Singleton<NHManager>.Instance.Insert<PetStatus>(petstatusObj);
                    petDict.Add(petObj.ID, petObj.PetID);

                    Singleton<NHManager>.Instance.Update(new RolePet() { RoleID = rolepet.RoleID, PetIDDict = Utility.Json.ToJson(petDict) });
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
            }
            else
            {
                    petDict = new Dictionary<int, int>();
                    petDict = Utility.Json.ToObject<Dictionary<int, int>>(rolepet.PetIDDict);

                    petObj = Singleton<NHManager>.Instance.Insert<Pet>(petObj);
                    petstatusObj.PetID = petObj.ID;
                    petstatusObj = Singleton<NHManager>.Instance.Insert<PetStatus>(petstatusObj);
                    petDict.Add(petObj.ID, petObj.PetID);
                    Singleton<NHManager>.Instance.Insert(new RolePet() { RoleID = rolepet.RoleID, PetIDDict = Utility.Json.ToJson(petDict) });
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaroleID);
        }
    }
}
