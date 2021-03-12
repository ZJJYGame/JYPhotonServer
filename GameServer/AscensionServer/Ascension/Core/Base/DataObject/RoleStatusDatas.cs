using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
   public class RoleStatusDatas
    {
        public int LevelID { get; set; }
        public int NextLevelID { get; set; }
        public int IsFinalLevel { get; set; }
        public int FreeAttributes { get; set; }
        public int RoleHP { get; set; }
        public int RoleMP { get; set; }
        public int RoleSoul { get; set; }
        public int BestBlood { get; set; }
        public int AttackSpeed { get; set; }
        public int AttackPhysical { get; set; }
        public int DefendPhysical { get; set; }
        public int AttackPower { get; set; }
        public int DefendPower { get; set; }
        public int PhysicalCritProb { get; set; }
        public int MagicCritProb { get; set; }
        public int ReduceCritProb { get; set; }
        public int PhysicalCritDamage { get; set; }
        public int MagicCritDamage { get; set; }
        public int ReduceCritDamage { get; set; }
        public int MoveSpeed { get; set; }
        public int RolePopularity { get; set; }
        public int ValueHide { get; set; }
        public int GongfaLearnSpeed { get; set; }
        public int MishuLearnSpeed { get; set; }
        public int Vitality { get; set; }
        public int ExpLevelUp { get; set; }
    }
}


