using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using RedisDotNet;
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
            string petstatusJson = Convert.ToString(Utility.GetValue(dict,(byte)ParameterCode.PetStatus));
            string petaptitudeJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.PetPtitude));

            var petstatusObj = Utility.Json.ToObject<PetStatus>(petstatusJson);
            var petaptitudeObj = Utility.Json.ToObject<PetaPtitudeDTO>(petstatusJson);
            NHCriteria nHCriteriapetstatus = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("PetID", petstatusObj.PetID);

            NHCriteria nHCriteriapetaptitude = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("PetID", petaptitudeObj.PetID);
            var petstatusTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<PetStatus>(nHCriteriapetstatus);
            var petaptitudeTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<PetaPtitude>(nHCriteriapetaptitude);
            if (petstatusTemp!=null)
            {
                petstatusTemp= petstatusObj;
            }
            if (petaptitudeTemp != null)
            {
                petaptitudeTemp= AddaPtitude(petaptitudeObj, petaptitudeTemp);
            }
            SetResponseData(() =>
            {
                SubDict.Add((byte)ParameterCode.PetStatus, Utility.Json.ToJson(petstatusTemp));
                SubDict.Add((byte)ParameterCode.PetPtitude, Utility.Json.ToJson(petaptitudeTemp));
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            });

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriapetstatus, nHCriteriapetaptitude);
        }

        #region 待删
        PetStatus AddStatus(PetStatus petStatusclient, PetStatus petStatusserver)
        {
            petStatusserver.PetAbilityPower += petStatusclient.PetAbilityPower;
            petStatusserver.PetAttackDamage += petStatusclient.PetAttackDamage;
            petStatusserver.PetHP += petStatusclient.PetHP;
            petStatusserver.PetMaxHP += petStatusclient.PetMaxHP;
            petStatusserver.PetMaxMP += petStatusclient.PetMaxMP;
            petStatusserver.PetMaxShenhun += petStatusclient.PetMaxShenhun;
            petStatusserver.PetMP += petStatusclient.PetMP;
            petStatusserver.PetResistanceAttack += petStatusclient.PetResistanceAttack;
            petStatusserver.PetResistancePower += petStatusclient.PetResistancePower;
            petStatusserver.PetShenhun += petStatusclient.PetShenhun;
            petStatusserver.PetShenhunDamage += petStatusclient.PetShenhunDamage;
            petStatusserver.PetShenhunResistance += petStatusclient.PetShenhunResistance;
            petStatusserver.PetSpeed += petStatusclient.PetSpeed;
            petStatusserver.PetTalent += petStatusclient.PetTalent;
            return petStatusserver;
        }

        PetaPtitude AddaPtitude(PetaPtitudeDTO petStatusclient, PetaPtitude petStatusserver)
        {
           
            foreach (var drugitem in petStatusclient.PetaptitudeDrug)
            {
                var drugDict = Utility.Json.ToObject<Dictionary<int,int>>(petStatusserver.PetaptitudeDrug);
                if (drugitem.Value>=10)
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
        #endregion
    }
}
