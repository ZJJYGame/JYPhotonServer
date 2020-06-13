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
    public class GetRolePetHandler : SyncRolePetSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            //AscensionServer._Log.Info(">>>>>>>>>>>>>》》》》》》》》》》》》>>获得宠物进来了");
            var dict = ParseSubDict(operationRequest);
            string rolepet = Convert.ToString(Utility.GetValue(dict,(byte)ObjectParameterCode.RolePet));

            var rolepetObj = Utility.ToObject<RolePet>(rolepet);
            NHCriteria nHCriteriaRolePet = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolepetObj.RoleID);
            
            var rolepets = Singleton<NHManager>.Instance.CriteriaGet<RolePet>(nHCriteriaRolePet);
            Utility.Assert.NotNull(rolepets.PetIDArray, () =>
             {
                 var rpetObj = Singleton<NHManager>.Instance.CriteriaGet<RolePet>(nHCriteriaRolePet);
                 string RolePetList = rpetObj.PetIDArray;
                 List<string> PetIDList;
                 List<Pet> petlist = new List<Pet>();
                 List<NHCriteria> nHCriteriasList = new List<NHCriteria>();
                 Utility.Assert.NotNull(RolePetList, () =>
                 {
                     PetIDList = new List<string>();
                     PetIDList = Utility.ToObject<List<string>>(RolePetList);
                     for (int i = 0; i < PetIDList.Count; i++)
                     {
                         NHCriteria nHCriteriapet = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", int.Parse(PetIDList[i]));
                         Pet petObj = Singleton<NHManager>.Instance.CriteriaGet<Pet>(nHCriteriapet);
                         petlist.Add(petObj);
                         nHCriteriasList.Add(nHCriteriapet);
                     }
                 }); SetResponseData(() =>
                 {
                     SubDict.Add((byte)ObjectParameterCode.RolePet, Utility.ToJson(petlist));
                     Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;

                 });
                 Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriasList);
             }, () => SetResponseData(()=>
             {
                 AscensionServer._Log.Info(">>>>>>>>>>>>>》》》》》》》》》》》》>>获得宠物失败");
                 SubDict.Add((byte)ObjectParameterCode.RolePet, Utility.ToJson(new List<string>()));
                 Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
             }));

            peer.SendOperationResponse(Owner.OpResponse,sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRolePet);
        }
    }
}
