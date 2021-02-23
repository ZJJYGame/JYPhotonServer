using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;
namespace AscensionServer
{
   public class GongFaStudyManager
    {
        public static bool AddGongFaJudge(int gongfaBookId,RoleDTO roleDTO,out CultivationMethodDTO gongfaTemp)
        {
            GameEntry. DataManager.TryGetValue<Dictionary<int, GongFaBook>>(out var gongfabookDict);
            gongfaTemp = CosmosEntry.ReferencePoolManager.Spawn<CultivationMethodDTO>();
            gongfabookDict.TryGetValue(gongfaBookId, out var gongFaBook);
            if (!Utility.Json.ToObject<List<int>>(roleDTO.RoleRoot).Contains(gongFaBook.Book_Property)&& roleDTO.RoleLevel< gongFaBook.Need_Level_ID)
            {
                return false;
            }
            gongfaTemp.CultivationMethodLevel = (short)gongFaBook.Need_Level_ID;
            gongfaTemp.CultivationMethodID = gongFaBook.Gongfa_ID;
            gongfaTemp.CultivationMethodLevelSkillArray = new List<int>();
            GameEntry. DataManager.TryGetValue<Dictionary<int, GongFa>>(out var gongfaDict);
            for (int i = 0; i < gongfaDict[gongfaTemp.CultivationMethodID].Skill_One.Count; i++)
            {
                gongfaTemp.CultivationMethodLevelSkillArray.Add(gongfaDict[gongfaTemp.CultivationMethodID].Skill_One[i]);
            }
            Utility.Debug.LogInfo("功法检测yzqData4" + gongfaDict[gongfaTemp.CultivationMethodID].Skill_One.Count);
            gongfaTemp.CultivationMethodLevelSkillArray.AddRange(gongfaDict[gongfaTemp.CultivationMethodID].Skill_One);

            return true;
        }

    }
}


