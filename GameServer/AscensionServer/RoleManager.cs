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
        public void AddRole(User role)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(role);
                    transaction.Commit();
                }
            }
        }


        public bool VerifyRole(string rolename, string gender)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                User user = session
                    .CreateCriteria(typeof(User))
                    .Add(Restrictions.Eq("Rolename", rolename))
                    .Add(Restrictions.Eq("Gender", gender))
                    .UniqueResult<User>();
                if (user == null) return false;
                return true;
            }
        }
    }
}
