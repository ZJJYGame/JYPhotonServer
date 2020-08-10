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
    public class AddWeaponSubHandler : SyncWeaponSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }


        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string weaponJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.GetWeapon));
            var weaponObj = Utility.Json.ToObject<WeaponDTO>(weaponJson);
            NHCriteria nHCriteriaweapon = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", weaponObj.RoleID);
            var weaponTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Weapon>(nHCriteriaweapon);
            int index;
            Dictionary<int, int> indexDict = new Dictionary<int, int>();
            Dictionary<int, WeaponStatusDTO> WeaponDict = new Dictionary<int, WeaponStatusDTO>();
            if (weaponTemp != null)
            {
                indexDict = Utility.Json.ToObject<Dictionary<int, int>>(weaponTemp.Weaponindex);
                if (indexDict.TryGetValue(weaponObj.WeaponID, out index))
                {
                    WeaponDict.Add(Convert.ToInt32(weaponObj.WeaponID + "" + index), weaponObj.WeaponStatusDict[weaponObj.WeaponID]);
                    weaponTemp.WeaponStatusDict = Utility.Json.ToJson(WeaponDict);
                    indexDict[weaponObj.WeaponID] += 1;
                    weaponTemp.Weaponindex = Utility.Json.ToJson(indexDict);
                    ConcurrentSingleton<NHManager>.Instance.Update(weaponTemp);
                }
            }
        }
    }
}
