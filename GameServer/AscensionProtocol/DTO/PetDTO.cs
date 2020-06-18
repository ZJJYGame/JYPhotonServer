using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class PetDTO: DataTransferObject
    {
        public virtual int ID { get; set; }
        public virtual byte PetLevel { get; set; }
        public virtual byte PetID { get; set; }
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
        }
    }
}
