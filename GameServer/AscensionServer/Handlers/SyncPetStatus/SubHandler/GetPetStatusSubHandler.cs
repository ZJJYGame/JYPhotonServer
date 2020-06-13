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
    public class GetPetStatusSubHandler : SyncPetStatusSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {

            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>同步宠物属性竟来了");
            var dict = ParseSubDict(operationRequest);
            string petstatus = Convert.ToString(Utility.GetValue(dict,(byte)ObjectParameterCode.PetStatus));
            var rolepetObj = Utility.ToObject<RolePet>(petstatus);
            List<PetStatus> petstatusList;
            NHCriteria nHCriteriarolepet = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolepetObj.RoleID);
            var petarray = Singleton<NHManager>.Instance.CriteriaGet<RolePet>( nHCriteriarolepet);
            if (petarray != null)
            {
                
                petstatusList = new List<PetStatus>();
                if (!string.IsNullOrEmpty(petarray.PetIDArray))
                {
                    foreach (var petid in Utility.ToObject<List<int>>(petarray.PetIDArray))
                    {
                        NHCriteria nHCriteriapetstatus = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", petid);

                        var petidarray = Singleton<NHManager>.Instance.CriteriaGet<PetStatus>(nHCriteriapetstatus);
                        petstatusList.Add(petidarray);
                        AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>同步宠物属性进来了"+ petidarray);
                    }
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    SubDict.Add((byte)ObjectParameterCode.PetStatus,Utility.ToJson(petstatusList));
                }
                else
                {
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                    SubDict.Add((byte)ObjectParameterCode.PetStatus, Utility.ToJson(new List<string>()));
                }
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse,sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriarolepet);
        }
    }
}
