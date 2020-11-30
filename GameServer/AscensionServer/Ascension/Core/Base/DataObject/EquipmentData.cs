using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class EquipmentData
    {
        public int Weapon_ID { get; set; }
        public string Weapon_Name { get; set; }
        public string Weapon_Describe { get; set; }
        public string Weapon_Icon { get; set; }
        public int Weapon_Quality { get; set; }
        public int Need_Level_ID { get; set; }
        public List<int> Weapon_Attack { get; set; }
        public int Value_Flow { get; set; }
        public int Weapon_Power { get; set; }
        public int Weapon_Durable { get; set; }
        public int Weapon_Skill { get; set; }
        public string Weapon_Effect { get; set; }
        public EquipType Weapon_Type { get; set; }

    }
}
