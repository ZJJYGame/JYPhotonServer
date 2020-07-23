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
    public class UpdatePetHandler : SyncPetSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);

            string petJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Pet));

            var petObj = Utility.Json.ToObject<Pet>(petJson);
            NHCriteria nHCriteriaPet = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", petObj.ID);
            int level;
            int exp;
            var pet = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Pet>(nHCriteriaPet);
            if (pet != null)
            {
                if (petObj.PetLevel!=0)
                {
                   
                    pet.PetExp = 0;
                    level = pet.PetLevel + petObj.PetLevel;
                    exp = pet.PetExp + petObj.PetExp;
                    ConcurrentSingleton<NHManager>.Instance.Update(new Pet()
                    {
                        ID= petObj.ID,PetID=petObj.PetID,PetExp= exp,PetLevel=(byte)level,PetIsBattle=petObj.PetIsBattle,PetSkillArray=petObj.PetSkillArray,PetName=petObj.PetName
                    });
                }
                else
                {
                    exp = pet.PetExp + petObj.PetExp;
                    ConcurrentSingleton<NHManager>.Instance.Update(new Pet()
                    {
                        ID = petObj.ID,
                        PetID = petObj.PetID,
                        PetExp = exp,
                        PetLevel = pet.PetLevel,
                        PetIsBattle = petObj.PetIsBattle,
                        PetSkillArray = petObj.PetSkillArray,
                        PetName = petObj.PetName

                    });
                }
                SetResponseData(() =>
                {
                    var sendPet = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Pet>(nHCriteriaPet);
                    SubDict.Add((byte)ParameterCode.Pet, Utility.Json.ToJson(sendPet));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }
            else
            {
                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>传过来的宠物状态为空" + petJson);
                //pet = Singleton<NHManager>.Instance.Insert<Pet>(pet);
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
            }

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaPet);
        }
    }
}
