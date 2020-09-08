using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RolePetDTO: DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual string PetIDDict { get; set; }
        public virtual int PetIsBattle { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            PetIDDict = null;
            PetIsBattle = 0;
        }
    }
}
