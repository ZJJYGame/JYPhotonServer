using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
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
            AscensionServer._Log.Info("更新宠物数据的请求进来了》》》》》》》》》》》》》》》》》");
            var dict = ParseSubDict(operationRequest);
            string petstatusJson = Convert.ToString(Utility.GetValue(dict,(byte)ObjectParameterCode.PetStatus));
            var petstatusObj = Utility.Json.ToObject<PetStatus>(petstatusJson);
            NHCriteria nHCriteriapetstatus = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("PetID", petstatusObj.PetID);
            var result = Singleton<NHManager>.Instance.Verify<PetStatus>(nHCriteriapetstatus);
            if (result)
            {
                Singleton<NHManager>.Instance.Update(petstatusObj);
                SetResponseData(() => Owner.OpResponse.ReturnCode = (short)ReturnCode.Success);
            }
            else
            {
                SetResponseData(() => Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail);
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
