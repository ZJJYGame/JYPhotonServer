using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RoleDTO:DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual byte RoleFaction { get; set; }
        public virtual bool RoleGender { get; set; }
        public virtual int RoleTalent { get; set; }
        public virtual string RoleRoot { get; set; }
        public virtual string RoleName { get; set; }
        public virtual int RoleLevel { get; set; }
        public virtual TeamInstructions teamInstructions { get; set; }
        public virtual TeamDTO teamDTO { get; set; } 
        public enum TeamInstructions
        {
            CreateTeam = 1,
            JoinTeam = 2,
            ApplyTeam = 3,
            RefusedTeam = 4,
        }
        public override void Clear()
        {
            RoleID = -1;
            RoleFaction = 0;
            RoleTalent = 0;
            RoleRoot = null;
            RoleName = null;
            RoleLevel = 0;
            teamInstructions = 0;
            teamDTO = null;
        }
        public override string ToString()
        {
            string str = "RoleID : "+ RoleID+ " ; RoleGender : "+ RoleGender+ " ; RoleName : "+ RoleName;
            return str;
        }
    }
}
