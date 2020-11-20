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
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Update;
        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string petptitudeJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.PetPtitude));

            var petaptitudeObj = Utility.Json.ToObject<PetaPtitudeDTO>(petptitudeJson);
            NHCriteria nHCriteriapetaptitude = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("PetID", petaptitudeObj.PetID);
            var petaptitudeTemp = NHibernateQuerier.CriteriaSelect<PetaPtitude>(nHCriteriapetaptitude);

            if (petaptitudeTemp != null)
            {   
                petaptitudeTemp = AddaPtitude(petaptitudeObj, petaptitudeTemp);
                NHibernateQuerier.Update(petaptitudeTemp);
                SetResponseParamters(() =>
                {
                    PetaPtitudeDTO petaPtitudeDTO = new PetaPtitudeDTO() {AttackphysicalAptitude= petaptitudeTemp.AttackphysicalAptitude,AttackspeedAptitude= petaptitudeTemp.AttackspeedAptitude,AttackpowerAptitude= petaptitudeTemp.AttackpowerAptitude,DefendphysicalAptitude= petaptitudeTemp.DefendphysicalAptitude,DefendpowerAptitude= petaptitudeTemp.DefendpowerAptitude,HPAptitude= petaptitudeTemp.HPAptitude,Petaptitudecol= petaptitudeTemp .Petaptitudecol,PetaptitudeDrug= Utility.Json.ToObject<Dictionary<int,int>>(petaptitudeTemp.PetaptitudeDrug),PetID= petaptitudeTemp .PetID,SoulAptitude= petaptitudeTemp .SoulAptitude};
                    subResponseParameters.Add((byte)ParameterCode.PetPtitude, Utility.Json.ToJson(petaPtitudeDTO));
                    operationResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }else
                operationResponse.ReturnCode = (short)ReturnCode.Fail;
            GameManager.ReferencePoolManager.Despawns(nHCriteriapetaptitude);
            return operationResponse;
        }

        PetaPtitude AddaPtitude(PetaPtitudeDTO petStatusclient, PetaPtitude petStatusserver)
        {
            var drugDict = Utility.Json.ToObject<Dictionary<int, int>>(petStatusserver.PetaptitudeDrug);
            int index;
            foreach (var drugitem in petStatusclient.PetaptitudeDrug)
            {
                Utility.Debug.LogInfo("收到的增加资质的请求" + drugitem.Key);
                if (drugDict.Count > 0)
                {
                    if (drugDict.TryGetValue(drugitem.Key, out index))
                    {
                        Utility.Debug.LogInfo("1收到的增加资质的请求");
                        petStatusserver.AttackphysicalAptitude += petStatusclient.AttackphysicalAptitude;
                        petStatusserver.AttackpowerAptitude += petStatusclient.AttackpowerAptitude;
                       
                        petStatusserver.AttackspeedAptitude += petStatusclient.AttackspeedAptitude;
                        petStatusserver.DefendphysicalAptitude += petStatusclient.DefendphysicalAptitude;
                        petStatusserver.DefendpowerAptitude += petStatusclient.DefendpowerAptitude;
                        petStatusserver.HPAptitude += petStatusclient.HPAptitude;
                        petStatusserver.SoulAptitude += petStatusclient.SoulAptitude;
                        petStatusserver.Petaptitudecol += petStatusclient.Petaptitudecol;
                        drugDict[drugitem.Key] += petStatusclient.PetaptitudeDrug[drugitem.Key];
                    }
                    else
                    {
                        Utility.Debug.LogInfo("1收到的增加资质的请求");
                        petStatusserver.AttackphysicalAptitude += petStatusclient.AttackphysicalAptitude;
                        petStatusserver.AttackpowerAptitude += petStatusclient.AttackpowerAptitude;
                        petStatusserver.AttackspeedAptitude += petStatusclient.AttackspeedAptitude;
                        petStatusserver.DefendphysicalAptitude += petStatusclient.DefendphysicalAptitude;
                        petStatusserver.DefendpowerAptitude += petStatusclient.DefendpowerAptitude;
                        petStatusserver.HPAptitude += petStatusclient.HPAptitude;
                        petStatusserver.SoulAptitude += petStatusclient.SoulAptitude;
                        petStatusserver.Petaptitudecol += petStatusclient.Petaptitudecol;
                        drugDict.Add(drugitem.Key, drugitem.Value);
                        Utility.Debug.LogInfo("2收到的增加资质的请求");
                    }
                }
                else
                {
                    Utility.Debug.LogInfo("2收到的增加资质的请求");
                    petStatusserver.AttackphysicalAptitude += petStatusclient.AttackphysicalAptitude;
                    petStatusserver.AttackpowerAptitude += petStatusclient.AttackpowerAptitude;
                   
                    petStatusserver.AttackspeedAptitude += petStatusclient.AttackspeedAptitude;
                    petStatusserver.DefendphysicalAptitude += petStatusclient.DefendphysicalAptitude;
                    petStatusserver.DefendpowerAptitude += petStatusclient.DefendpowerAptitude;                   
                    petStatusserver.HPAptitude += petStatusclient.HPAptitude;
                    petStatusserver.SoulAptitude += petStatusclient.SoulAptitude;
                    petStatusserver.Petaptitudecol += petStatusclient.Petaptitudecol;
                    drugDict.Add(drugitem.Key, drugitem.Value);
                }
            }
            petStatusserver.PetaptitudeDrug = Utility.Json.ToJson(drugDict);
            return petStatusserver;
        }
    }
}
