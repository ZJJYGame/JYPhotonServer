using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class PetAptitudeData
    {
        public int PetID { get; set; }
        public int NaturalHP { get; set; }
        public int NaturalMP { get; set; }
        public int NaturalSoul { get; set; }
        public int NaturalAttackSpeed { get; set; }
        public int NaturalAttackPhysical { get; set; }
        public int NaturalDefendPhysical { get; set; }
        public int NaturalAttackPower { get; set; }
        public int NaturalDefendPower { get; set; }
        public int FlowValue { get; set; }
        public List<int> NaturalGrowUp { get; set; }
        public List<int> SkillArray { get; set; }
    }
}
