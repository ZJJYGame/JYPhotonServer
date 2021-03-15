using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /*功法书的实体类*/
    [Serializable]
    [ConfigData]
    public class GongFa
    {
        public int Gongfa_ID { get; set; }
        public string Gongfa_Quarity { get; set; }
        public int Gongfa_Property { get; set; }
        public int Need_Gongfa_ID { get; set; }
        public int Need_Level_ID { get; set; }
        public int Max_Level_ID { get; set; }
        public List<int> Skill_One { get; set; }
        public List<int> Skill_One_At_Level { get; set; }
        public int Role_HP { get; set; }
        public int Role_MP { get; set; }
        public int Role_Soul { get; set; }
        public int Best_Blood { get; set; }
        public int Attact_Speed { get; set; }
        public int Attact_Physical { get; set; }
        public int PhysicalCritProb { get; set; }
        public int MagicCritProb { get; set; }
        public int ReduceCritProb { get; set; }
        public int PhysicalCritDamage { get; set; }
        public int MagicCritDamage { get; set; }
        public int ReduceCritDamage { get; set; }
        public int Defend_Physical { get; set; }
        public int Attact_Power { get; set; }
        public int Defend_Power { get; set; }
        public int Move_Speed { get; set; }
        public int Role_Popularity { get; set; }
        public int Value_Hide { get; set; }
        public int Gongfa_LearnSpeed { get; set; }
        public int Mishu_LearnSpeed { get; set; }
    }
}


