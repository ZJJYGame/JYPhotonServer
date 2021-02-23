using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
   public class RolePetMap:ClassMap<RolePet>
    {
        public RolePetMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("role_id");
            Map(x => x.PetIDDict).Column("pet_id_dict");
            Map(x => x.PetIsBattle).Column("pet_isbattle");
            Table("role_pet");
        }
    }
}


