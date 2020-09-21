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
   public class GetPetHandler: SyncPetSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);

            string petJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Pet));

            var petObj = Utility.Json.ToObject<Pet>(petJson);
            NHCriteria nHCriteriaPet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", petObj.ID);
            var pet = NHibernateQuerier.CriteriaSelect<Pet>(nHCriteriaPet);
            if (pet!=null)
            {
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.Pet, Utility.Json.ToJson(pet));
                    Owner.OpResponseData.ReturnCode = (byte)ReturnCode.Success;
                });
            }else
                Owner.OpResponseData.ReturnCode = (byte)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaPet);

        }

       

    }
}
