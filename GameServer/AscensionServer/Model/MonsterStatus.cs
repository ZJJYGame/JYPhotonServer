using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    public class MonsterStatus
    {
        public virtual int MonsterID { get; set; }
        public virtual short MonsterSpeed { get; set; }
        public virtual short MonsterAttackDamage { get; set; }
        public virtual short MonsterResistanceAttack { get; set; }
        public virtual short MonsterPowerDamage { get; set; }
        public virtual short MonsterResistancePower { get; set; }
        public virtual int MonsterMaxHP { get; set; }
        public virtual int MonsterHP { get; set; }
        public virtual int MonsterMaxMP { get; set; }
        public virtual int MonsterMP { get; set; }
        public virtual string MonsterTalent { get; set; }
        public virtual ushort MonsterShenshi { get; set; }
        public virtual int MonsterShenhunDamage { get; set; }
        public virtual int MonsterShenhunResistance { get; set; }
    }
}
