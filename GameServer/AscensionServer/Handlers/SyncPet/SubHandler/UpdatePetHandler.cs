using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UpdatePetHandler : SyncPetSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);

            string petJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Pet));

            var petObj = Utility.Json.ToObject<Pet>(petJson);
            NHCriteria nHCriteriaPet = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", petObj.ID);       
            var pet = Singleton<NHManager>.Instance.CriteriaSelect<Pet>(nHCriteriaPet);
            if (pet != null)
            {
                Singleton<NHManager>.Instance.Update<Pet>(petObj);
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.Pet, Utility.Json.ToJson(pet));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }
            else
            {
                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>传过来的宠物状态为空" + petJson);
                pet = Singleton<NHManager>.Instance.Insert<Pet>(pet);
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
            }

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaPet);
        }
    }
}
