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
            var demonicalDict = Utility.Json.ToObject<Dictionary<int,DemonicSoulEntity>>(demonicSoul.DemonicSouls);
            DemonicSoulEntity demonicSoulEntity = new DemonicSoulEntity();
            if (indexDict.ContainsKey(soulid))
            {
                demonicSoulEntity.UniqueID = indexDict[soulid] + 1;
                indexDict[soulid] += 1;
            }
            else
            {
                demonicSoulEntity.UniqueID =0;
                indexDict.Add(soulid, 0);
            }
            demonicSoulEntity.GlobalID = soulid;
            GameManager.CustomeModule<PetStatusManager>().AddDemonicSoul (soulid,out var skillList);
            demonicSoulEntity.Skills = skillList;
            demonicalDict.Add(demonicSoulEntity.UniqueID, demonicSoulEntity);

            demonicSoul.DemonicSoulIndex = Utility.Json.ToJson(indexDict);
            demonicSoul.DemonicSouls = Utility.Json.ToJson(demonicalDict);

            NHibernateQuerier.Update(demonicSoul);


            var ringObj = GameManager.ReferencePoolManager.Spawn<RingDTO>();
            ringObj.RingItems = new Dictionary<int, RingItemsDTO>();
            ringObj.RingItems.Add(demonicSoulEntity.UniqueID, new RingItemsDTO());
            var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteria);
            var nHCriteriaRingID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ringServer.RingIdArray);
            InventoryManager.AddDataCmd(demonicSoulEntity.UniqueID, ringObj, nHCriteriaRingID);
        }

        public void CompoundDemonical(List<int>soulList,DemonicSoul demonicSoul,int roleid,NHCriteria nHCriteria)
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
            if ((demonicSoulData[soulList[0]].MergeSuccessRate * soulList.Count)<random.Next(0,101))
            {
                S2CDemonicalMessage(roleid,null,ReturnCode.Fail);
            }
            else
            {
                AddDemonical(roleid, demonicSoul, demonicSoulData[soulList[0]].MergeDemonicSoulID, nHCriteria);
                S2CDemonicalMessage(roleid, null, ReturnCode.Success);
            }

            for (int i = 0; i < soulList.Count; i++)
            {
                ringObj.RingItems = new Dictionary<int, RingItemsDTO>();
                ringObj.RingItems.Add(soulList[i], new RingItemsDTO());
                InventoryManager.RemoveCmd(soulList[i], ringObj, nHCriteriaRingID);
            }
        }

        public void S2CDemonicalMessage(int roleid,string message,ReturnCode returnCode)
        {
            OperationData opData = new OperationData();
            opData.DataMessage = Utility.Json.ToJson(message);
            opData.OperationCode = (byte)OperationCode.SyncDemonical;
            opData.ReturnCode = (byte)returnCode;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleid, opData);
        }

    }
}
