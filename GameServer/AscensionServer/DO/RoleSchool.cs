using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer.Model
{
    [Serializable]
    public class RoleSchool : DataObject
    {
        public virtual int RoleID { get; set; }
        public virtual Dictionary<int, int> RoleJoiningSchool { get; set; }
        public virtual Dictionary<int, int> RoleJoinedSchool { get; set; }


        public override void Clear()
        {
            RoleID = 0;
            RoleJoiningSchool=null;
            RoleJoinedSchool=null;
        }
    }
}
