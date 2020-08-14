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

            int index=1;
            Dictionary<int, int> indexDict = new Dictionary<int, int>();
            Dictionary<int, List<int>> WeaponDict = new Dictionary<int, List<int>>();
            if (weaponTemp != null)
            {
                if (!string.IsNullOrEmpty(weaponTemp.Weaponindex)&& !weaponTemp.Weaponindex.Equals("{}"))
                {

                    AscensionServer._Log.Info("添加后的武器数值为" + weaponTemp.Weaponindex);
                    indexDict = Utility.Json.ToObject<Dictionary<int, int>>(weaponTemp.Weaponindex);
                    WeaponDict= Utility.Json.ToObject<Dictionary<int, List<int>>>(weaponTemp.WeaponStatusDict);
                    AscensionServer._Log.Info("添加后的武器数值为" + indexDict.Count);
                    if (indexDict.TryGetValue(weaponObj.WeaponID, out index))
                    {
                        AscensionServer._Log.Info("1添加后的武器数值为" + index);
                        WeaponDict.Add(Convert.ToInt32(weaponObj.WeaponID + "" + (index+1)), weaponObj.WeaponStatusDict[weaponObj.WeaponID]);
                        index = Convert.ToInt32(weaponObj.WeaponID + "" + (index + 1));
                        weaponTemp.WeaponStatusDict = Utility.Json.ToJson(WeaponDict);
                        indexDict[weaponObj.WeaponID] += 1;
                        weaponTemp.Weaponindex = Utility.Json.ToJson(indexDict);
                        ConcurrentSingleton<NHManager>.Instance.Update(weaponTemp);
                    }
                    else
                    {
                        AscensionServer._Log.Info("2添加后的武器数值为" + weaponTemp.RoleID);
                        WeaponDict.Add(Convert.ToInt32(weaponObj.WeaponID + "" + 1), weaponObj.WeaponStatusDict[weaponObj.WeaponID]);
                        index = Convert.ToInt32(weaponObj.WeaponID + "" + 1);
                        weaponTemp.WeaponStatusDict = Utility.Json.ToJson(WeaponDict);
                        indexDict.Add(weaponObj.WeaponID, 1);
                        weaponTemp.Weaponindex = Utility.Json.ToJson(indexDict);
                        ConcurrentSingleton<NHManager>.Instance.Update(weaponTemp);
                    }
                }
                else
                {
                    AscensionServer._Log.Info("3添加后的武器数值为" + weaponTemp.RoleID);
                    WeaponDict.Add(Convert.ToInt32(weaponObj.WeaponID + "" + 1), weaponObj.WeaponStatusDict[weaponObj.WeaponID]);
                    index = Convert.ToInt32(weaponObj.WeaponID + "" + 1);
                    weaponTemp.WeaponStatusDict = Utility.Json.ToJson(WeaponDict);
                    indexDict.Add(weaponObj.WeaponID, 1);
                    weaponTemp.Weaponindex = Utility.Json.ToJson(indexDict);
                    ConcurrentSingleton<NHManager>.Instance.Update(weaponTemp);
            }
            SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.GetWeapon, Utility.Json.ToJson(weaponTemp));
                    SubDict.Add((byte)ParameterCode.GetWeaponindex, Utility.Json.ToJson(index));
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
