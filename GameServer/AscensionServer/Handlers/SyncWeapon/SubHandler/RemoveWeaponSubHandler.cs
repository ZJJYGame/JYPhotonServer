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
    public class RemoveWeaponSubHandler : SyncWeaponSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Remove;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string weaponJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.GetWeapon));
            var weaponObj = Utility.Json.ToObject<WeaponDTO>(weaponJson);

            NHCriteria nHCriteriaweapon = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", weaponObj.RoleID);
            var weapontemp = NHibernateQuerier.CriteriaSelect<Weapon>(nHCriteriaweapon);

            if (weapontemp != null)
            {
                var WeaponDict = Utility.Json.ToObject<Dictionary<int, List<int>>>(weapontemp.WeaponStatusDict);
                foreach (var item in weaponObj.WeaponStatusDict)
                {
                    WeaponDict.Remove(item.Key);
                }
                weapontemp.WeaponStatusDict = Utility.Json.ToJson(WeaponDict);
                NHibernateQuerier.Update(weapontemp);
                SetResponseParamters(() =>
                {
                    operationResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }
            else
            {
                SetResponseParamters(() =>
                {
                    operationResponse.ReturnCode = (byte)ReturnCode.Fail;
                });
            }

            GameManager.ReferencePoolManager.Despawns(nHCriteriaweapon);
            return operationResponse;
        }
    }
}
