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
using RedisDotNet;
namespace AscensionServer
{
    public class UpdatePetHandler : SyncPetSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }
        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);

            string petJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Pet));

            var petObj = Utility.Json.ToObject<Pet>(petJson);
            NHCriteria nHCriteriaPet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", petObj.ID);
            var pet = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Pet>(nHCriteriaPet);
            if (pet != null && RedisHelper.Hash.HashExistAsync("Pet", petObj.ID.ToString()).Result)
            {
                if (petObj.PetLevel!=0)
                {

                   pet.PetLevel += petObj.PetLevel;
                   pet.PetExp =petObj.PetExp;
                    ConcurrentSingleton<NHManager>.Instance.Update(pet);
                  await  RedisHelper.Hash.HashSetAsync<Pet>("Pet", pet.ID.ToString(), pet);
                }
                else
                {
                     pet.PetExp += petObj.PetExp;
                    ConcurrentSingleton<NHManager>.Instance.Update(pet);
                   await RedisHelper.Hash.HashSetAsync<Pet>("Pet", pet.ID.ToString(), pet);
                }
                SetResponseData(() =>
                {
                    Utility.Debug.LogInfo(">>>>>>>>>>>>>>>>>>>>穿回去的宠物经验" + petJson);
                    SubDict.Add((byte)ParameterCode.Pet, Utility.Json.ToJson(pet));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }
            else
            {
                Utility.Debug.LogInfo(">>>>>>>>>>>>>>>>>>>>传过来的宠物状态为空" + petJson);
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
            }

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaPet);
        }
    }
}
