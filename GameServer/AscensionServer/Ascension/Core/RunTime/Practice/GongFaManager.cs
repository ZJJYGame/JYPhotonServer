using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionData;
using AscensionProtocol.DTO;
using UnityEngine;

namespace AscensionServer
{
    [CustomeModule]
   public class GongFaManager : Module<GongFaManager>
    {
        public Dictionary<int, GongFa> gongfaDataDict = new Dictionary<int, GongFa>();

        public GongFaManager()
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, GongFa>>(out gongfaDataDict);
        }

        public List<int> GetSkillList(int level,int gongfaid)
        {
            var result = GetGngFa(gongfaid);
            if (result !=null&& result.Need_Level_ID <= level)
                return result.Skill_One;
            else
                return null;
        }

        public GongFa GetGngFa(int gongfaid)
        {
            var result = gongfaDataDict.TryGetValue(gongfaid, out GongFa gongFa);
            if (result)
                return gongFa;
            else
                return null;
        }


    }
}
