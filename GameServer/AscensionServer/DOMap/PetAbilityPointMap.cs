using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer
{
    public class PetAbilityPointMap : ClassMap<PetAbilityPoint>
    {
        public PetAbilityPointMap()
        {
            Id(x => x.ID).GeneratedBy.Assigned().Column("id");
            Map(x => x.SlnNow).Column("slnnow");
            Map(x => x.IsUnlockSlnThree).Column("is_unlock_slnthree");
            Map(x => x.AbilityPointSln).Column("ability_point_sln");
            Table("pet_ability_point");
        }
    }
}
