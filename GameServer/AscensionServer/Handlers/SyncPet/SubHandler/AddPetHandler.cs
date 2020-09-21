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

            string petJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Pet));

            var petObj = Utility.Json.ToObject<Pet>(petJson);
            NHCriteria nHCriteriaPet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("PetID", petObj.PetID);
            var PetObj = NHibernateQuerier.CriteriaSelect<Pet>(nHCriteriaPet);
            if (!string.IsNullOrEmpty(petJson))
            {
                petObj = NHibernateQuerier.Insert(petObj);
                Utility.Debug.LogInfo(">>>>>>>>>>>>>>>>>>>>>>>>>添加宠物进来了》》》》》》》》》》》》》》》");
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.Pet, petObj);
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
    }

