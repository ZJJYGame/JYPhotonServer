using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer
{
    public class AlchemyMap:ClassMap<Alchemy>
    {
        public AlchemyMap()
        {
            Id(x => x.RoleID).GeneratedBy.Increment().Column("roleid");
            Map(x => x.AlchemyStatusDict).Column("alchemy_status_dict");
            Table("alchemy");
        }
    }
}
