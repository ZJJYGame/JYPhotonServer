using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionProtocol;
using AscensionServer.Model;
using Cosmos;
using Protocol;
namespace AscensionServer
{
    [CustomeModule]
    public partial class DemonicSoulManager : Module<DemonicSoulManager>
    {
        public void AddDemonical(int roleid,DemonicSoul demonicSoul,int soulid, NHCriteria nHCriteria)
        {
            var indexDict = Utility.Json.ToObject<Dictionary<int,int>>(demonicSoul.DemonicSoulIndex);
            var demonicalDict = Utility.Json.ToObject<Dictionary<int, DemonicSoulEntity>>(demonicSoul.DemonicSouls);
            DemonicSoulEntity demonicSoulEntity = new DemonicSoulEntity();
            if (indexDict.ContainsKey(soulid))
            {
                demonicSoulEntity.UniqueID = int.Parse(soulid.ToString() + (indexDict[soulid] + 1));
                indexDict[soulid] += 1;
            }
            else
            {
                demonicSoulEntity.UniqueID =int.Parse( soulid.ToString()+0);
                indexDict.Add(soulid, 0);
            }
            demonicSoulEntity.GlobalID = soulid;
            GameManager.CustomeModule<PetStatusManager>().AddDemonicSoul (soulid,out var skillList);
            demonicSoulEntity.Skills = new List<int>();
            demonicSoulEntity.Skills = skillList;
            demonicalDict.Add(demonicSoulEntity.UniqueID, demonicSoulEntity);

            demonicSoul.DemonicSoulIndex = Utility.Json.ToJson(indexDict);
            demonicSoul.DemonicSouls = Utility.Json.ToJson(demonicalDict);

            NHibernateQuerier.Update(demonicSoul);

            DemonicSoulDTO demonicSoulDTO = new DemonicSoulDTO();
            demonicSoulDTO.RoleID = demonicSoul.RoleID;
            demonicSoulDTO.CompoundList = new List<int>();
            demonicSoulDTO.OperateType = DemonicSoulOperateType.Add;
            demonicSoulDTO.DemonicSouls = demonicalDict;
            Utility.Debug.LogInfo("yzqData添加妖灵精魄" + Utility.Json.ToJson(demonicSoulDTO));
            S2CDemonicalMessage(roleid, Utility.Json.ToJson(demonicSoulDTO), ReturnCode.Success);

            Utility.Debug.LogInfo("yzqData添加妖灵精魄" + Utility.Json.ToJson(demonicSoul));
            InventoryManager.AddNewItem(roleid, demonicSoulEntity.UniqueID,1);
        }

        public async void CompoundDemonical(List<int>soulList,DemonicSoul demonicSoul,int roleid,NHCriteria nHCriteria)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, DemonicSoulData>> (out var demonicSoulData);

            var demonicalDict = Utility.Json.ToObject<Dictionary<int, DemonicSoulEntity>>(demonicSoul.DemonicSouls);

            var ringObj = GameManager.ReferencePoolManager.Spawn<RingDTO>();

            var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteria);
            var nHCriteriaRingID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ringServer.RingIdArray);
            if (soulList.Count==0)
            {
                S2CDemonicalMessage(roleid, null, ReturnCode.Fail);
                return;
            }

            for (int i = 0; i < soulList.Count; i++)
            {
                if (demonicalDict[soulList[i]].GlobalID != demonicalDict[soulList[0]].GlobalID)
                {
                    S2CDemonicalMessage(roleid, null, ReturnCode.Fail);
                    return;
                }
                if (!InventoryManager.VerifyIsExist(soulList[i],nHCriteriaRingID))
                {
                    S2CDemonicalMessage(roleid, null, ReturnCode.Fail);
                    return;
                }
            }

            if (!demonicSoulData[demonicalDict[soulList[0]].GlobalID].MergeNumber.Contains(soulList.Count))
            {
                S2CDemonicalMessage(roleid, null, ReturnCode.Fail);
                return;
            }

            Random random = new Random();
            if ((demonicSoulData[demonicalDict[soulList[0]].GlobalID].MergeSuccessRate * soulList.Count)<random.Next(0,101))
            {
                S2CDemonicalMessage(roleid,null,ReturnCode.Fail);
            }
            else
            {
                AddDemonical(roleid, demonicSoul, demonicSoulData[demonicalDict[soulList[0]].GlobalID].MergeDemonicSoulID, nHCriteria);
            }

            for (int i = 0; i < soulList.Count; i++)
            {
                InventoryManager.Remove(roleid, soulList[i]);
                demonicalDict.Remove(soulList[i]);
            }
            demonicSoul.DemonicSouls = Utility.Json.ToJson(demonicalDict);
          await  NHibernateQuerier.UpdateAsync(demonicSoul);
        }

        public void GetDemonicSoul(int roleid, DemonicSoul demonicSoul)
        {
            var demonicalDict = Utility.Json.ToObject<Dictionary<int, DemonicSoulEntity>>(demonicSoul.DemonicSouls);
            DemonicSoulDTO demonicSoulDTO = new DemonicSoulDTO();
            demonicSoulDTO.RoleID = demonicSoul.RoleID;
            demonicSoulDTO.CompoundList = new List<int>();
            demonicSoulDTO.OperateType = DemonicSoulOperateType.Get;
            demonicSoulDTO.DemonicSouls = demonicalDict;
            Utility.Debug.LogInfo("yzqData添加妖灵精魄" + Utility.Json.ToJson(demonicSoulDTO));
            S2CDemonicalMessage(roleid, Utility.Json.ToJson(demonicSoulDTO), ReturnCode.Success);
        }

        public void S2CDemonicalMessage(int roleid,string message,ReturnCode returnCode)
        {
            OperationData opData = new OperationData();
            opData.DataMessage = message;
            opData.OperationCode = (byte)OperationCode.SyncDemonical;
            opData.ReturnCode = (byte)returnCode;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleid, opData);
        }

    }
}
