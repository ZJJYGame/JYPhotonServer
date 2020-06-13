using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using AscensionServer.Model;
namespace AscensionServer.ModelMap
{
    public class OffLineTimeMap : ClassMap<OffLineTime>
    {
        public OffLineTimeMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.OffTime).Column("offtime");
            Table("offlinetime");
        }
    }
}
