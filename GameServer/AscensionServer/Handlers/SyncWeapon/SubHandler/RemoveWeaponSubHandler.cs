﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;

namespace AscensionServer
{
    public class RemoveWeaponSubHandler : SyncWeaponSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Remove;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string weaponJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.GetWeapon));
            var weaponObj = Utility.Json.ToObject<WeaponDTO>(weaponJson);

            NHCriteria nHCriteriaweapon = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", weaponObj.RoleID);
            var weapontemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Weapon>(nHCriteriaweapon);

            if (weapontemp != null)
            {
                var WeaponDict = Utility.Json.ToObject<Dictionary<int, List<int>>>(weapontemp.WeaponStatusDict);
                foreach (var item in weaponObj.WeaponStatusDict)
                {
                    WeaponDict.Remove(item.Key);
                }
                weapontemp.WeaponStatusDict = Utility.Json.ToJson(WeaponDict);
                ConcurrentSingleton<NHManager>.Instance.Update(weapontemp);
                SetResponseData(() =>
                {
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }
            else
            {
                SetResponseData(() =>
                {
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
                });
            }

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaweapon);



        }
    }
}