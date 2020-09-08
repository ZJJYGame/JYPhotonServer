using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using RedisDotNet;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    public class UpdatePetStatusSubHandler : SyncPetStatusSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string petstatusJson = Convert.ToString(Utility.GetValue(dict,(byte)ParameterCode.PetStatus));
            Utility.Debug.LogInfo("收到的宠物状态数据" + petstatusJson);

            var petstatusObj = Utility.Json.ToObject<PetStatus>(petstatusJson);
            NHCriteria nHCriteriapetstatus = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("PetID", petstatusObj.PetID);


            var petstatusTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<PetStatus>(nHCriteriapetstatus);

            if (petstatusTemp!=null)
            {

                petstatusTemp = petstatusObj;
                ConcurrentSingleton<NHManager>.Instance.Update<PetStatus>(petstatusTemp);
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.PetStatus, Utility.Json.ToJson(petstatusTemp));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
            {
                SetResponseData(() =>
                {
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriapetstatus);
        }


    }
}
