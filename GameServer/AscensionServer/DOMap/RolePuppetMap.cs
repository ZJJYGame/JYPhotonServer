using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer
{
   public class RolePuppetMap : ClassMap<RolePuppet>
    {
        public RolePuppetMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("role_id");
            Map(x => x.PuppetDict).Column("puppetdict");
            Map(x => x.IsBattle).Column("isbattle");
            Table("role_puppet");
        }
    }
}
