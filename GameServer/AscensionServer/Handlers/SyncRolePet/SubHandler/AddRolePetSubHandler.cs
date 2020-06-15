using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;

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

            string rpJson = Convert.ToString(Utility.GetValue(dict,(byte)ObjectParameterCode.RolePet));
            string pJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.Pet));
            string psJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.PetStatus));

            var rolepetObj = Utility.ToObject<RolePet>(rpJson);
            var petObj = Utility.ToObject<Pet>(pJson);
            var petstatusObj = Utility.ToObject<PetStatus>(psJson);
            NHCriteria nHCriteriaroleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolepetObj.RoleID);
            var rolepet = Singleton<NHManager>.Instance.CriteriaGet<RolePet>(nHCriteriaroleID);
            Dictionary<int, int> petDict;
            Utility.Assert.NotNull(rolepet.PetIDDict,()=>
            {
                petDict = new Dictionary<int, int>();
                petDict = Utility.ToObject<Dictionary<int, int>>(rolepet.PetIDDict);
                petstatusObj.PetID = petObj.ID;
                petObj = Singleton<NHManager>.Instance.Add<Pet>(petObj);
                petstatusObj = Singleton<NHManager>.Instance.Add<PetStatus>(petstatusObj);
                petDict.Add(petObj.ID, petObj.PetID);

                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            },()=>
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            });
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaroleID);
        }
    }
}
