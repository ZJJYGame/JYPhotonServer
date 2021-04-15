using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer
{
   public class PuppetIndividualMap : ClassMap<PuppetIndividual>
    {
        public PuppetIndividualMap()
        {
            Id(x => x.ID).Column("id").GeneratedBy.Increment();
            Map(x => x.MP).Column("mp");
            Map(x => x.PuppetDurable).Column("puppet_durable");
            Map(x => x.PuppetID).Column("puppetid");
            Map(x => x.AttackPhysical).Column("attack_physical");
            Map(x => x.DefendPower).Column("defend_power");
            Map(x => x.AttackPower).Column("attack_Power");
            Map(x => x.AttackSpeed).Column("attack_speed");
            Map(x => x.DefendPhysical).Column("defend_physical");
            Map(x => x.PuppetDurableMax).Column("puppet_durablemax");        
            Map(x => x.Skills).Column("skills");
            Table("puppet_Individual");

        }
    }
}
