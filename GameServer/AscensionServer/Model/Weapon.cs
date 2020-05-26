using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class Weapon
    {
        public virtual int ID { get; set; }
        public virtual byte WeaponID { get; set; }
        public virtual byte WeaponLevel { get; set; }
        public virtual int WeaponExp { get; set; }
    }
}
