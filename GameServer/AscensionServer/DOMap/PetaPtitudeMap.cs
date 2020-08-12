using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer
{
  public  class PetaPtitudeMap: ClassMap<PetaPtitude>
    {
        public PetaPtitudeMap()
        {
            Id(x => x.PetID).GeneratedBy.Assigned().Column("petid");
            Map(x => x.SoulAptitude).Column("soul_aptitude");
            Map(x => x.Petaptitudecol).Column("petaptitudecol");
            Map(x => x.MPAptitude).Column("mp_aptitude");
            Map(x => x.HPAptitude).Column("hp_aptitude");
            Map(x => x.DefendpowerAptitude).Column("defendpower_aptitude");
            Map(x => x.DefendphysicalAptitude).Column("defendphysical_aptitude");
            Map(x => x.AttackspeedAptitude).Column("attackspeed_aptitude");
            Map(x => x.AttacksoulAptitude).Column("attacksoul_aptitude");
            Map(x => x.AttackpowerAptitude).Column("attackpower_aptitude");
            Map(x => x.AttackphysicalAptitude).Column("attackphysical_aptitude");
            Map(x => x.DefendsoulAptitude).Column("defendsoul_aptitude");
            Table("petaptitude");
        }
    }
}
