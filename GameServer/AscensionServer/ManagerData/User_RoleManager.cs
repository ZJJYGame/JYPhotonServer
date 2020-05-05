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
    public class User_RoleManager : Singleton<User_RoleManager> ,IUser_RoleManager
    {
        public  void AddStr(User_Role str)
        {

            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(str);
                    transaction.Commit();
                }
            }

        }

        public  void UpdateStr(User_Role str)
        {

            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Update(str);
                    transaction.Commit();
                }
            }

        }

        public  string GetArray(string uuid)
        {

            using (ISession session = NHibernateHelper.OpenSession())
            {
                User_Role user = session
                    .CreateCriteria(typeof(User_Role))
                    .Add(Restrictions.Eq("UUID", uuid))
                    .UniqueResult<User_Role>();
                if (user == null)
                {
                    return "";
                }
                return user.Role_Id_Array;
            }
        }

    }
}
