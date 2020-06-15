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
            List<PetStatus> petstatusList;
            NHCriteria nHCriteriarolepet = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolepetObj.RoleID);
            var petarray = Singleton<NHManager>.Instance.CriteriaGet<RolePet>( nHCriteriarolepet);
            List<int> petstatuslist;
            if (petarray != null)
            {

                petstatusList = new List<PetStatus>();
                if (!string.IsNullOrEmpty(petarray.PetIDArray))
                {
                    //AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>同步宠物属性竟来了" + petarray.PetIDArray);
                    petstatuslist = new List<int>();
                    petstatuslist = Utility.Json.ToObject<List<int>>(petarray.PetIDArray);
                    for (int i = 0; i < petstatuslist.Count; i++)
                    {
                        NHCriteria nHCriteriapetstatus = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", petstatuslist[i]);
                        AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>同步宠物属性进来了" + petstatuslist.Count);
                        var petidarray = Singleton<NHManager>.Instance.CriteriaGet<PetStatus>(nHCriteriapetstatus);


                        petstatusList.Add(petidarray);
                    }
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    SubDict.Add((byte)ObjectParameterCode.PetStatus,Utility.Json.ToJson(petstatusList));
                }
                else
                {
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                    SubDict.Add((byte)ObjectParameterCode.PetStatus, Utility.Json.ToJson(new List<string>()));
                }
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse,sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriarolepet);
        }
    }
}
