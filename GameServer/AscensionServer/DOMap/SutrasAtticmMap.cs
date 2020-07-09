using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer
{
   public  class SutrasAtticmMap:ClassMap<SutrasAttic>
    {
        public SutrasAtticmMap()
        {
            Id(x => x.ID).GeneratedBy.Increment().Column("id");
            Map(x => x.SutrasAmountDict).Column("sutras_amount_dict");
            Map(x => x.SutrasRedeemedDictl).Column("sutras_redeemed_dictl");
            Table("sutrasattic");
        }
    }
}
