using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using AscensionServer.Model;
namespace AscensionServer
{
    public class PetMap:ClassMap<Pet>
    {
        public PetMap()
        {
            Id(x => x.ID).Column("id").GeneratedBy.Increment();
            Map(x => x.PetExp).Column("pet_exp");
            Map(x => x.PetID).Column("pet_id");
            Map(x => x.PetLevel).Column("pet_level");
            Map(x => x.PetName).Column("pet_name");
            Map(x => x.PetSkillArray).Column("pet_skill_array");
            Map(x => x.PetExtraSkill).Column("pet_extra_skill");
            Table("pet");
        }
    }
}
