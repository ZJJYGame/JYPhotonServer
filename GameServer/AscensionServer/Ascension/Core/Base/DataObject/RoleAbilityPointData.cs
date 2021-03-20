﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
   public class RoleAbilityPointData
    {
        public string CoefficientType { get; set; }
        public float HPAttribute { get; set; }
        public float MPAttribute { get; set; }
        public float PowerDefendAttribute { get; set; }
        public float PhysicalDefendAttribute { get; set; }
        public float AttackPowerAttribute { get; set; }
        public float SoulAttribute { get; set; }
        public float AttackSpeedAttribute { get; set; }
        public float AttackPhysicalAttribute { get; set; }
        public float BestBloodAttribute { get; set; }
        public float MoveSpeedAttribute { get; set; }
    }
}
