using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos;
namespace AscensionProtocol.DTO
{
    [Serializable]
    public class PetDTO: DataTransferObject
    {
        public virtual int ID { get; set; }
        public virtual int PetLevel { get; set; }
        public virtual int PetID { get; set; }
        public virtual int PetExp { get; set; }
        public virtual string PetName { get; set; }
        public virtual List<int> PetSkillArray { get; set; }
        public virtual Dictionary<int,List<int>> DemonicSoul { get; set; }
        public override void Clear()
        {
            ID = -1;
            PetLevel = 0;
            PetID = 0;
            PetExp = 0;
            PetName = null;
            PetSkillArray.Clear();
            DemonicSoul.Clear();
        }
    }
}
