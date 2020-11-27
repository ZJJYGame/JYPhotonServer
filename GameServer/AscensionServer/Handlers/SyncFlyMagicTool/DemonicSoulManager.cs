using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
   public class DemonicSoulManager
    {
        public void AddDemonical(int roleid,DemonicSoul demonicSoul,int soulid)
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
        }


        public void CompoundDemonical(List<int>soulList,DemonicSoul demonicSoul)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, DemonicSoulData>> (out var demonicSoulData);

            var demonicalDict = Utility.Json.ToObject<Dictionary<int, DemonicSoulEntity>>(demonicSoul.DemonicSouls);

            for (int i = 0; i < soulList.Count; i++)
            {
                if (demonicalDict[soulList[i]].GlobalID == demonicalDict[soulList[0]].GlobalID)
                {

                }
            }
        }
    }
}
