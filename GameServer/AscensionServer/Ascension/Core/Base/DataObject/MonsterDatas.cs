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
        public int ID { get; set; }
        public int Monster_ID { get; set; }
        public string Monster_name { get; set; }
        public string Monster_describe { get; set; }
        public string Monster_level { get; set; }
        public int Role_HP { get; set; }
        public int Role_MP { get; set; }
        public int Role_soul { get; set; }
        public float Attact_speed { get; set; }
        public int Attact_physical { get; set; }
        public int Defence_physical { get; set; }
        public int Attact_power { get; set; }
        public int Defence_power { get; set; }
        public int Attact_soul { get; set; }
        public int Defence_soul { get; set; }
        public int UP_double { get; set; }
        public int Down_double { get; set; }
        public int Value_flow { get; set; }
        public  int PhysicalCritProb { get; set; }
        public  int MagicCritProb { get; set; }
        public  int ReduceCritProb { get; set; }
        public  int PhysicalCritDamage { set; get; }
        public  int MagicCritDamage { get; set; }
        public  int ReduceCritDamage { get; set; }
        public int Alert_area { get; set; }
        public int Move_speed { get; set; }
        public int Value_hide { get; set; }
        public int Best_blood { get; set; }
        public List<int> Skill_Array { get; set; }
        public List<int> Drop_Array { get; set; }
        public List<int> Drop_Rate { get; set; }
        public string Monster_Icon { get; set; }
        public string Moster_Model { get; set; }
        public int Pet_ID { get; set; }
        public byte Pet_Level_ID { get; set; }
    }
}
