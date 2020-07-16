using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    public class RoleAdventureSkillDTO : DataTransferObject
    {
        public int RoleID { get; set; }
        public bool IsInUse { get; set; }
        public int SkillID  { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            IsInUse = false;
            SkillID = 0;
        }
    }
}
