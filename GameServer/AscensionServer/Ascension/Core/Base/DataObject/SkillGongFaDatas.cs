using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /*技能 功法 实体类*/
    [Serializable]
    [ConfigData]
    public class SkillGongFaDatas
    {
        public int index { get; set; }
        public int Skill_ID { get; set; }
        public string Skill_Name { get; set; }
        public string Skill_Describe { get; set; }
        public int Skill_Type { get; set; }
        public int Attack_Number { get; set; }
        public int Attack_Value { get; set; }
        public List<int> Attack_Factor { get; set; }
        public int Add_Buff_Probability { get; set; }
        public int Buff_ID { get; set; }
        public int Buff_Time { get; set; }
        public int Fight_Type { get; set; }
        public TargetType Attack_Target { get; set; }
        public int Cost_Number { get; set; }
        public int Cost_Type { get; set; }
        public DamageType Skill_Property { get; set; }
        public int Skill_CD { get; set; }
        public string Skill_Icon { get; set; }
        public string Skill_Effect { get; set; }
        public string Animation_Path { get; set; }
        public Battle_MoveType Move_Type { get; set; }
        public AttackProcess_Type AttackProcess_Type { get; set; }
    }
}
