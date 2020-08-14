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
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string petptitudeJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.PetPtitude));
            var petaptitudeObj = Utility.Json.ToObject<PetaPtitudeDTO>(petptitudeJson);
            NHCriteria nHCriteriaweapon = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", petaptitudeObj.PetID);
            var petaptitudeTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<PetaPtitude>(nHCriteriaweapon);

            if (petaptitudeTemp != null)
            {
                petaptitudeTemp = AddaPtitude(petaptitudeObj, petaptitudeTemp);
            }
            SetResponseData(() =>
            {
                SubDict.Add((byte)ParameterCode.PetPtitude, Utility.Json.ToJson(petaptitudeTemp));
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            });
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
