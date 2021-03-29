using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer.DOMap
{
    public class AllianceDongFuMap : ClassMap<AllianceDongFu>
    {
        public AllianceDongFuMap()
        {
            Id(x => x.AllianceID).GeneratedBy.Assigned().Column("allianceid");
            Map(x => x.SpiritRangeID).Nullable().Column("spirit_sphygmus_id");
            Map(x => x.PreemptOne).Nullable().Column("preempt_one");
            Map(x => x.PreemptTow).Nullable().Column("preempt_tow");
            Map(x => x.PreemptThree).Nullable().Column("preempt_three");
            Map(x => x.PreemptFour).Nullable().Column("preempt_four");
            Map(x => x.PreemptFive).Nullable().Column("preempt_five");
            Table("alliance_dongfu");
        }
    }
}
