using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using NHibernate.Linq.Clauses;
using AscensionProtocol.DTO;
using Renci.SshNet.Security;
using Cosmos;

namespace AscensionServer
{
    public class UpdatePetaPtitudeSubHandler : SyncPetaPtitudeSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string petptitudeJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.PetPtitude));
            AscensionServer._Log.Info("收到的增加资质的请求"+ petptitudeJson);
            var petaptitudeObj = Utility.Json.ToObject<PetaPtitudeDTO>(petptitudeJson);
            NHCriteria nHCriteriapetaptitude = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", petaptitudeObj.PetID);
            var petaptitudeTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<PetaPtitude>(nHCriteriapetaptitude);

            if (petaptitudeTemp != null)
            {
                petaptitudeTemp = AddaPtitude(petaptitudeObj, petaptitudeTemp);
                SetResponseData(() =>
                {
                    AscensionServer._Log.Info("发送回去的增加资质的" + Utility.Json.ToJson(petaptitudeTemp));
                    SubDict.Add((byte)ParameterCode.PetPtitude, Utility.Json.ToJson(petaptitudeTemp));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriapetaptitude);
        }

        PetaPtitude AddaPtitude(PetaPtitudeDTO petStatusclient, PetaPtitude petStatusserver)
        {

            foreach (var drugitem in petStatusclient.PetaptitudeDrug)
            {
                var drugDict = Utility.Json.ToObject<Dictionary<int, int>>(petStatusserver.PetaptitudeDrug);
                if (drugitem.Value >= 10)
                {
                    return petStatusserver;
                }
                else
                {
                    petStatusserver.AttackphysicalAptitude += petStatusclient.AttackphysicalAptitude;
                    petStatusserver.AttackpowerAptitude += petStatusclient.AttackpowerAptitude;
                    petStatusserver.AttacksoulAptitude += petStatusclient.AttacksoulAptitude;
                    petStatusserver.AttackspeedAptitude += petStatusclient.AttackspeedAptitude;
                    petStatusserver.DefendphysicalAptitude += petStatusclient.DefendphysicalAptitude;
                    petStatusserver.DefendpowerAptitude += petStatusclient.DefendpowerAptitude;
                    petStatusserver.DefendsoulAptitude += petStatusclient.DefendsoulAptitude;
                    petStatusserver.HPAptitude += petStatusclient.HPAptitude;
                    petStatusserver.MPAptitude += petStatusclient.MPAptitude;
                    petStatusserver.SoulAptitude += petStatusclient.SoulAptitude;
                    petStatusserver.Petaptitudecol += petStatusclient.Petaptitudecol;
                    drugDict[drugitem.Key] += drugDict[drugitem.Key];
                }
            }

            return petStatusserver;
        }
    }
}
