/*
*Author : xianrenZhang
*Since 	:2020-04-28
*Description  : 用户角色中间表管理者
*/
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
    public class UserRoleManager : Singleton<UserRoleManager> ,IUserRoleManager
    {
        public  void AddStr(UserRole str)
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

        public  void UpdateStr(UserRole str)
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
                UserRole user = session
                    .CreateCriteria(typeof(UserRole))
                    .Add(Restrictions.Eq("UUID", uuid))
                    .UniqueResult<UserRole>();
                if (user == null)
                {
                    return "";
                }
                return user.Role_Id_Array;
            }
        }

    }
}
