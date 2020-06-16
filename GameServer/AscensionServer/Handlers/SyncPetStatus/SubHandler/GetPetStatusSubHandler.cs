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
    public class GetPetStatusSubHandler : SyncPetStatusSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string petstatus = Convert.ToString(Utility.GetValue(dict,(byte)ObjectParameterCode.PetStatus));
            var rolepetObj = Utility.Json.ToObject<RolePet>(petstatus);
            NHCriteria nHCriteriarolepet = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolepetObj.RoleID);
            var petarray = Singleton<NHManager>.Instance.CriteriaGet<RolePet>( nHCriteriarolepet);

            Utility.Assert.NotNull(petarray.PetIDDict, () =>
             {
                 string petdict =petarray.PetIDDict;
                 Dictionary<int, int> petIDList;
                 List<PetStatus> petList = new List<PetStatus>();
                 List<NHCriteria> nHCriteriasList = new List<NHCriteria>();
                 Utility.Assert.NotNull(petdict, () =>
                 {
                     petIDList = new Dictionary<int, int>();
                     petIDList = Utility.Json.ToObject<Dictionary<int,int>>(petdict);
                     foreach (var petid in petIDList)
                     {
                         NHCriteria nHCriteriapet = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("PetID", petid.Key);
                         var petstatusObj = Singleton<NHManager>.Instance.CriteriaGet<PetStatus>(nHCriteriapet);
                         petList.Add(petstatusObj);
                         nHCriteriasList.Add(nHCriteriapet);
                     }
                 }); SetResponseData(() =>
                 {
                     SubDict.Add((byte)ObjectParameterCode.PetStatus, Utility.Json.ToJson(petList));
                     Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                 });
                 Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriasList);
             }, () => SetResponseData(()=>
             {
                 AscensionServer._Log.Info(">>>>>>>>>>>>>》》》》》》》》》》》》>>获得宠物数据失败");
                 SubDict.Add((byte)ObjectParameterCode.PetStatus, Utility.Json.ToJson(new List<string>()));
                 Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
             }));
            peer.SendOperationResponse(Owner.OpResponse,sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriarolepet);
        }
    }
}
