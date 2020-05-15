using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using AscensionServer.Model;
namespace AscensionServer
{
    public class MonsterMap:ClassMap<Monster>
    {
        public MonsterMap()
        {
            Id(x => x.ID).Column("id").GeneratedBy.Increment();
            Map(x => x.MonsterExp).Column("monster_exp");
            Map(x => x.MonsterID).Column("monster_id");
            Map(x => x.MonsterLevel).Column("monster_level");
            Map(x => x.MonsterName).Column("monster_name");
            Map(x => x.MonsterSkillArray).Column("monster_skill_array");
            Table("monster");
        }
    }
}
