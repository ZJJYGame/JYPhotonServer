using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using AscensionServer.Model;
namespace AscensionServer
{
    public class UserMap:ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.UUID).GeneratedBy.UuidHex("N").Column("uuid");
            Map(x => x.Account).Unique().Column("account");
            Map(x => x.Password).Column("password");
            Table("user");
        }
    }
}
