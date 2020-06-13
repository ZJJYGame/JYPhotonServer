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

            var petObj = Utility.ToObject<Pet>(petJson);
            NHCriteria nHCriteriaPet = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", petObj.ID);

            
        }

       

    }
}
