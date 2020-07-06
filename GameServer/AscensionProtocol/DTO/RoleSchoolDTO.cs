using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RoleSchoolDTO : DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual Dictionary<int, int> RoleJoiningSchool { get; set; }
        public virtual Dictionary<int, int> RoleJoinedSchool { get; set; }


        public override void Clear()
        {
            RoleID = 0;
            RoleJoiningSchool.Clear();
            RoleJoinedSchool.Clear();
        }
    }
}
