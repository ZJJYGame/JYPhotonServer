using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
   public class RolePetMap:ClassMap<RolePet>
    {
        public RolePetMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("role_id");
            Map(x => x.PetIDArray).Column("pet_id_array");
            Table("role_pet");
        }
    }
}
