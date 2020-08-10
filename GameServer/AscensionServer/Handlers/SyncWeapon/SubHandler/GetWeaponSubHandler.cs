using System;
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
    public class GetWeaponSubHandler : SyncWeaponSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string weaponJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.GetWeapon));
            var weaponObj = Utility.Json.ToObject<WeaponDTO>(weaponJson);
            NHCriteria nHCriteriaweapon= ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", weaponObj.RoleID);
            var weapontemp= ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Weapon>(nHCriteriaweapon);

            if (weapontemp!=null)
            {
                var WeaponDict = Utility.Json.ToObject<Dictionary<int, WeaponStatusDTO>>(weapontemp.WeaponStatusDict);
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.GetWeapon, Utility.Json.ToJson(WeaponDict));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaweapon);
        }
    }
}
