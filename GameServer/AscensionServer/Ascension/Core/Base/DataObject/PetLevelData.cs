using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class PetLevelData
    {
        public int PetLevelID { get; set; }
        public int NextLevelID { get; set; }
        public bool IsFinalLevel { get; set; }
        public int FreeAttributes { get; set; }
        public int PetHP { get; set; }
        public int PetMP { get; set; }
        public float Petsoul { get; set; }
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
        public int ExpLevelUp { get; set; }
    }
}
