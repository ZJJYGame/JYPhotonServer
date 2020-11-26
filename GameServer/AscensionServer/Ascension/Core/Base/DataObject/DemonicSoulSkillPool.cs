using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
   public class DemonicSoulSkillPool
    {
        public int PetSkillLevel { get; set; }
        public List<int> PetSoulSkillPool { get; set; }
    }
}
