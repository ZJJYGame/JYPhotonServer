using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class PuppetUnitMap : ClassMap<PuppetUnit>
    {
        public PuppetUnitMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.UnitIndesDict).Column("unit_index");
            Map(x => x.PuppetUnitInfoDict).Column("puppetunit_status_dict");
            Table("puppetunit");
        }
    }
}
