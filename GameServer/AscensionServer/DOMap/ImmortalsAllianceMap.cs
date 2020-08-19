using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer.DOMap
{
    public class ImmortalsAllianceMap: ClassMap<ImmortalsAlliance>
    {
        public ImmortalsAllianceMap()
        {
            Id(x => x.ID).Column("id").GeneratedBy.Assigned();
            Map(x => x.ImmortalsallianceID).Column("immortalsalliance_id");
            Map(x => x.ImmortalsallianceLevel).Column("immortalsalliance_level");
            Map(x => x.ImmortalsallianceMaster).Column("immortalsalliance_master");
            Map(x => x.ImmortalsallianceName).Column("immortalsalliance_name");
            Map(x => x.immortalsallianceNumberPeople).Column("immortalsalliance_number_people");
            Map(x => x.ImmortalsalliancePeopleMax).Column("immortalsalliance_people_max");
            Map(x => x.IsApplyfor).Column("isapplyfor");
            Table("immortalsalliance");
        }
    }
}
