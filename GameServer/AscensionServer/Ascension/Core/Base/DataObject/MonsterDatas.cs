using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /*怪物的实体类*/
    [Serializable]
    [ConfigData]
    public class MonsterDatas
    {
        public int Monster_ID { get; set; }
        public string Monster_Name { get; set; }
        public string Monster_Describe { get; set; }
        public string Monster_Level { get; set; }
        public int Monster_Gig_Level { get; set; }
        public int Monster_HP { get; set; }
        public int Monster_MP { get; set; }
        public int Monster_Soul { get; set; }
        public float Attact_Speed { get; set; }
        public int Attact_Physical { get; set; }
        public int Defend_Physical { get; set; }
        public int Attact_Power { get; set; }
        public int Defend_Power { get; set; }
        public int Value_Flow { get; set; }
        public  int PhysicalCritProb { get; set; }
        public  int MagicCritProb { get; set; }
        public  int ReduceCritProb { get; set; }
        public  int PhysicalCritDamage { set; get; }
        public  int MagicCritDamage { get; set; }
        public  int ReduceCritDamage { get; set; }
        public int Alert_Area { get; set; }
        public int Move_Speed { get; set; }
        public bool IsHide { get; set; }
        public int Value_Hide { get; set; }
        public int Best_Blood { get; set; }
        public List<int> Skill_Array { get; set; }
        public List<int> Drop_Array { get; set; }
        public List<int> Drop_Rate { get; set; }
        public string Monster_Icon { get; set; }
        public string Moster_Model { get; set; }
        public int Pet_ID { get; set; }
        public byte Pet_Level_ID { get; set; }
    }
}
