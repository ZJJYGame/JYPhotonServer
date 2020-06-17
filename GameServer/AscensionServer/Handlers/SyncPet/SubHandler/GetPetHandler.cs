﻿using System;
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
   public class GetPetHandler: SyncPetSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);

            string petJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.Pet));

            var petObj = Utility.Json.ToObject<Pet>(petJson);
            NHCriteria nHCriteriaPet = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", petObj.ID);
            var pet = Singleton<NHManager>.Instance.CriteriaSelect<Pet>(nHCriteriaPet);
            if (pet!=null)
            {
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ObjectParameterCode.Pet, Utility.Json.ToJson(pet));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }else
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaPet);

        }

       

    }
}
