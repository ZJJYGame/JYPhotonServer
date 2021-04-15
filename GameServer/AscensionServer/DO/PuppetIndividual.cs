﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class PuppetIndividual: DataObject
    {
        public virtual int ID { set; get; }
        public virtual int HP { set; get; }
        public virtual int MP { set; get; }
        public virtual int PuppetID { set; get; }
        public virtual int AttackPhysical { set; get; }
        public virtual int AttackPower { set; get; }
        public virtual int DefendPhysical { set; get; }
        public virtual int DefendPower { set; get; }
        public virtual int AttackSpeed { set; get; }
        public virtual int PuppetDurable { set; get; }
        public virtual int PuppetDurableMax { set; get; }
        public virtual string Skills { set; get; }

        public PuppetIndividual()
        {
            ID = 0;
            HP = 0;
            MP = 0;
            PuppetID = 0;
            AttackPhysical = 0;
            AttackPower = 0;
            DefendPhysical = 0;
            DefendPower = 0;
            AttackSpeed = 0;
            PuppetDurable = 0;
            PuppetDurableMax = 0;
            Skills ="[]";
        }

        public override void Clear()
        {
          
        }
    }
}
