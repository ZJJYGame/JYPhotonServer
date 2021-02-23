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
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;
        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string petJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Pet));
            var petObj = Utility.Json.ToObject<Pet>(petJson);
            NHCriteria nHCriteriaPet = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", petObj.ID);
            var pet = NHibernateQuerier.CriteriaSelect<Pet>(nHCriteriaPet);
            if (pet!=null)
            {
                SetResponseParamters(() =>
                {
                    subResponseParameters.Add((byte)ParameterCode.Pet, Utility.Json.ToJson(pet));
                    operationResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }else
                operationResponse.ReturnCode = (byte)ReturnCode.Fail;
            CosmosEntry.ReferencePoolManager.Despawns(nHCriteriaPet);
            return operationResponse;
        }
    }
}


