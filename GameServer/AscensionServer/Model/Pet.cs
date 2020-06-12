using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class Pet
    {
        public virtual int ID { get; set; }
        public virtual byte PetLevel { get; set; }
        public virtual byte PetID { get; set; }
        public virtual int PetExp { get; set; }
        public virtual string PetName { get; set; }
        public virtual string PetSkillArray { get; set; }
    }
}
