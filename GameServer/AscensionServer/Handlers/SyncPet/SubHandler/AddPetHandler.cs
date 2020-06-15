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
    public class AddPetHandler : SyncPetSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();    
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);

            string petJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.Pet));

            var petObj = Utility.Json.ToObject<Pet>(petJson);
            NHCriteria nHCriteriaPet = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("PetID", petObj.PetID);

            var PetObj = Singleton<NHManager>.Instance.CriteriaGet<Pet>(nHCriteriaPet);
            Utility.Assert.NotNull(petJson,()=> 
            {
                petObj= Singleton<NHManager>.Instance.Add(petObj);
                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>>添加宠物进来了》》》》》》》》》》》》》》》");
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ObjectParameterCode.Pet, petObj);
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            },()=> SetResponseData(() =>
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }));
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
    }

