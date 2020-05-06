/*
*Author : xianrenZhang
*Since 	:2020-04-28
*Description  : 角色管理类
*/
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
    public class RoleManager : NHManager
    {
        //public Role AddRole(Model.Role role)
        //{
        //    using (ISession session = NHibernateHelper.OpenSession())
        //    {
        //        using (ITransaction transaction = session.BeginTransaction())
        //        {
        //            session.Save(role);
        //            transaction.Commit();
        //            return session.Get<Role>(role.RoleId);
        //        }
        //    }
        //}
        public string GetRoleId(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                Role role = session
                    .CreateCriteria(typeof(Role))
                    .Add(Restrictions.Eq("RoleId", id))
                    .UniqueResult<Role>();

                if (role == null)
                    return "";
                return role.RoleName;
            }
        }

        public bool VerifyRole(string rolename)//角色名字
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                Role role = session
                    .CreateCriteria(typeof(Role))
                    .Add(Restrictions.Eq("RoleName", rolename))
                    .UniqueResult<Role>();
                if (role == null) return false;
                return true;
            }
        }

        public Role GetByRolename(string rolename)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {

                Role role = session.CreateCriteria(typeof(Role)).Add(Restrictions.Eq("RoleName", rolename)).UniqueResult<Role>();
                return role;
            }
        }

        public ICollection<Role> GetAllRoles()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                IList<Role> roles = session.CreateCriteria(typeof(Role)).List<Role>();
                return roles;
            }
        }

        //public IList<RoleGongFa> GetGongFaList()
        //{
        //    ISession session = NHibernateHelper.OpenSession();
        //    return session.QueryOver<RoleGongFa>().List();
        //}

        public IList<Role> GetRoleList()
        {
            ISession session = NHibernateHelper.OpenSession();
            return session.QueryOver<Role>().List();
        }

        //public IList<RoleLevel> GetRoleLevelList()
        //{
        //    ISession session = NHibernateHelper.OpenSession();
        //    return session.QueryOver<RoleLevel>().List();
        //}
    }
}
