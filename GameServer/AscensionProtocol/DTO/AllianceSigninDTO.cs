using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class AllianceSigninDTO : DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual int AllianceID { get; set; }
        public virtual int Popularity { get; set; }
        public virtual int RoleContribution { get; set; }
        public virtual int AllianceSpiritStone { get; set; }
        public virtual bool IsSignin { get; set; }
        public override void Clear()
        {
            RoleID = 0;
            AllianceID = 0;
            Popularity = 0;
            RoleContribution = 0;
            AllianceSpiritStone = 0;
            IsSignin = false;
        }
    }
}
