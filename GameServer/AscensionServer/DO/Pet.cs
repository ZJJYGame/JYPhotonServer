using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class Pet:DataObject
    {
        public Pet()
        {
            PetLevel = 1;
            PetID = 8001;
            PetExp = 0;
            PetName = "蒜头王八";
            PetSkillArray = "21001";
            PetIsBattle = false;
        }
        public virtual int ID { get; set; }
        public virtual byte PetLevel { get; set; }
        public virtual int PetID { get; set; }
        public virtual int PetExp { get; set; }
        public virtual string PetName { get; set; }
        public virtual string PetSkillArray { get; set; }
        public virtual bool PetIsBattle { get; set; }
        public override void Clear()
        {
            ID = -1;
            PetLevel = 0;
            PetID = 0;
            PetExp = 0;
            PetName = null;
            PetSkillArray = null;
            PetIsBattle = false;
        }
    }
}
