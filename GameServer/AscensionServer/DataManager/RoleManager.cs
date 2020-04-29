using AscensionServer.Manager;
using AscensionServer.Model;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class RoleManager : IRoleManager
    {
        public IList<RoleGongFa> GetGongFaList()
        {
            ISession session = NHibernateHelper.OpenSession();
            return session.QueryOver<RoleGongFa>().List();
        }

        public IList<Role> GetRoleList()
        {
            ISession session = NHibernateHelper.OpenSession();
            return session.QueryOver<Role>().List();
        }
        public IList<RoleLevel> GetRoleLevelList()
        {
            ISession session = NHibernateHelper.OpenSession();
            return session.QueryOver<RoleLevel>().List();
        }
    }
}
