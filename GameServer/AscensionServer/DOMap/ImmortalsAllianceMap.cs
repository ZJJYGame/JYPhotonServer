﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer.DOMap
{
    public class ImmortalsAllianceMap: ClassMap<ImmortalsAlliance>
    {
        public ImmortalsAllianceMap()
        {
            Id(x => x.RoleID).Column("roleid").GeneratedBy.Assigned();
            Map(x => x.AllianceID).Column("alliance_id");
            Map(x => x.AllianceJob).Column("alliance_job");
            Map(x => x.Reputation).Column("reputation");
            Table("role_alliance");
        }
    }
}
