using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
   public class RoleMonsterMap:ClassMap<RoleMonster>
    {
        public RoleMonsterMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("role_id");
            Map(x => x.MonsterIDArray).Column("monster_id_array");
            Table("role_monster");
        }
    }
}
