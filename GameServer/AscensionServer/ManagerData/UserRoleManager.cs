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
    public class UserRoleManager : NHManager
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

        public UserRole GetByUUID(string uuid)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                /*
                ICriteria criteria= session.CreateCriteria(typeof(User));
                criteria.Add(Restrictions.Eq("Username", username));//添加一个查询条件,第一个参数表示对哪个属性(字段）做限制，第二个表示值为多少
                User user = criteria.UniqueResult<User>();
                */
                UserRole user = session.CreateCriteria(typeof(UserRole)).Add(Restrictions.Eq("UUID", uuid)).UniqueResult<UserRole>();
                return user;

            }
        }


        //public T GetUniqueEqual<T,K>(K key )
        //{

        //}
    }
}
