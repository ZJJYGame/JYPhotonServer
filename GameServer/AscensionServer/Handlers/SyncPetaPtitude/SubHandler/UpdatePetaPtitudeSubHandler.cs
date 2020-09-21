﻿using System;
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

            var petaptitudeObj = Utility.Json.ToObject<PetaPtitudeDTO>(petptitudeJson);
            NHCriteria nHCriteriapetaptitude = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("PetID", petaptitudeObj.PetID);
            var petaptitudeTemp = NHibernateQuerier.CriteriaSelect<PetaPtitude>(nHCriteriapetaptitude);

            if (petaptitudeTemp != null)
            {   
                petaptitudeTemp = AddaPtitude(petaptitudeObj, petaptitudeTemp);
                NHibernateQuerier.Update(petaptitudeTemp);
                SetResponseData(() =>
                {
                    PetaPtitudeDTO petaPtitudeDTO = new PetaPtitudeDTO() {AttackphysicalAptitude= petaptitudeTemp.AttackphysicalAptitude,AttacksoulAptitude= petaptitudeTemp.AttacksoulAptitude,AttackspeedAptitude= petaptitudeTemp.AttackspeedAptitude,AttackpowerAptitude= petaptitudeTemp.AttackpowerAptitude,DefendphysicalAptitude= petaptitudeTemp.DefendphysicalAptitude,DefendpowerAptitude= petaptitudeTemp.DefendpowerAptitude,HPAptitude= petaptitudeTemp.HPAptitude,DefendsoulAptitude= petaptitudeTemp.DefendsoulAptitude,MPAptitude= petaptitudeTemp .MPAptitude,Petaptitudecol= petaptitudeTemp .Petaptitudecol,PetaptitudeDrug= Utility.Json.ToObject<Dictionary<int,int>>(petaptitudeTemp.PetaptitudeDrug),PetID= petaptitudeTemp .PetID,SoulAptitude= petaptitudeTemp .SoulAptitude};
                    SubDict.Add((byte)ParameterCode.PetPtitude, Utility.Json.ToJson(petaPtitudeDTO));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriapetaptitude);
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
                        petStatusserver.AttacksoulAptitude += petStatusclient.AttacksoulAptitude;
                        petStatusserver.AttackspeedAptitude += petStatusclient.AttackspeedAptitude;
                        petStatusserver.DefendphysicalAptitude += petStatusclient.DefendphysicalAptitude;
                        petStatusserver.DefendpowerAptitude += petStatusclient.DefendpowerAptitude;
                        petStatusserver.DefendsoulAptitude += petStatusclient.DefendsoulAptitude;
                        petStatusserver.HPAptitude += petStatusclient.HPAptitude;
                        petStatusserver.MPAptitude += petStatusclient.MPAptitude;
                        petStatusserver.SoulAptitude += petStatusclient.SoulAptitude;
                        petStatusserver.Petaptitudecol += petStatusclient.Petaptitudecol;
                        drugDict[drugitem.Key] += petStatusclient.PetaptitudeDrug[drugitem.Key];
                    }
                    else
                    {
                        Utility.Debug.LogInfo("1收到的增加资质的请求");
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
                        drugDict.Add(drugitem.Key, drugitem.Value);
                        Utility.Debug.LogInfo("2收到的增加资质的请求");
                    }
                }
                else
                {
                    Utility.Debug.LogInfo("2收到的增加资质的请求");
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
                    drugDict.Add(drugitem.Key, drugitem.Value);
                }
            }
            petStatusserver.PetaptitudeDrug = Utility.Json.ToJson(drugDict);
            return petStatusserver;
        }
    }
}
