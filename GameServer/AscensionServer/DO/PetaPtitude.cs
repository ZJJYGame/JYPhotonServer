using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class PetaPtitude : DataObject
    {
        public virtual int PetID { get; set; }
        public virtual int HPAptitude { get; set; }
        public virtual int MPAptitude { get; set; }
        public virtual int SoulAptitude { get; set; }
        public virtual int AttackspeedAptitude { get; set; }
        public virtual int AttackphysicalAptitude { get; set; }
        public virtual int DefendphysicalAptitude { get; set; }
        public virtual int AttackpowerAptitude { get; set; }
        public virtual int DefendpowerAptitude { get; set; }
        public virtual int AttacksoulAptitude { get; set; }
        public virtual int DefendsoulAptitude { get; set; }
        public virtual int Petaptitudecol { get; set; }

        public PetaPtitude()
        {
            HPAptitude = 0;
            MPAptitude = 0;
            SoulAptitude = 0;
            AttackspeedAptitude = 0;
            AttackphysicalAptitude = 0;
            DefendphysicalAptitude = 0;
            AttackpowerAptitude = 0;
            DefendpowerAptitude = 0;
            AttacksoulAptitude = 0;
            DefendsoulAptitude = 0;
            Petaptitudecol = 0;
        }

        public override void Clear()
        {
            PetID = -1;
            HPAptitude = 0;
            MPAptitude = 0;
            SoulAptitude = 0;
            AttackspeedAptitude = 0;
            AttackphysicalAptitude = 0;
            DefendphysicalAptitude = 0;
            AttackpowerAptitude = 0;
            DefendpowerAptitude = 0;
            AttacksoulAptitude = 0;
            DefendsoulAptitude = 0;
            Petaptitudecol = 0;
        }
    }
}
