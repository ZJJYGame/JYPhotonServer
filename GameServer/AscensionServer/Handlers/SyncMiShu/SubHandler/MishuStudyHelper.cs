﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;

namespace AscensionServer
{
   public class MishuStudyHelper
    {
        public static bool AddMishuJuge(int mishuBookID, RoleDTO roleDTO, out MiShuDTO misuhTemp)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, MishuBook>>(out var mishubookDict);
            misuhTemp = GameManager.ReferencePoolManager.Spawn<MiShuDTO>();
            mishubookDict.TryGetValue(mishuBookID, out var mishuBook);
            if (!Utility.Json.ToObject<List<int>>(roleDTO.RoleRoot).Contains(mishuBook.Book_Property) && roleDTO.RoleLevel < mishuBook.Need_Level_ID)
            {
                return false;
            }
            misuhTemp.MiShuID = mishuBook.Mishu_ID;
            misuhTemp.MiShuAdventureSkill = new List<int>();
            misuhTemp.MiShuSkillArry = new List<int>();
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, MiShuData>>(out var mishuDict);
            for (int i = 0; i < mishuDict[misuhTemp.MiShuID].mishuSkillDatas[0].Skill_Array_One.Count; i++)
            {
                misuhTemp.MiShuSkillArry.Add(mishuDict[misuhTemp.MiShuID].mishuSkillDatas[0].Skill_Array_One[0]);
            }
            for (int i = 0; i < mishuDict[misuhTemp.MiShuID].mishuSkillDatas[0].Skill_Array_Two.Count; i++)
            {
                misuhTemp.MiShuAdventureSkill.Add(mishuDict[misuhTemp.MiShuID].mishuSkillDatas[0].Skill_Array_Two[0]);
            }

            Utility.Debug.LogInfo("秘术检测yzqData4" + misuhTemp.MiShuSkillArry.Count);
            //misuhTemp.CultivationMethodLevelSkillArray.AddRange(gongfaDict[misuhTemp.CultivationMethodID].Skill_One);

            return true;
        }
    }
}