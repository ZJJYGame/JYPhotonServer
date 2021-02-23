using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class PetSkillBookData
    {
        public int PetSkillBookID { get; set; }
        public int PetSkillID { get; set; }
    }
}


