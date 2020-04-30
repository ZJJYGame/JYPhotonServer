using AscensionProtocol.Model;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;

namespace AscensionServer
{
    public class UserManager : IUserManager
    {
        public void Add(Model.User user)
        {
            /*第一种
            ISession session = NHibernateHelper.OpenSession();
            session.Save(user);
            session.Close();
            */
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(user);
                    transaction.Commit();
                }
            }
        }

        public ICollection<User> GetAllUsers()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                IList<User> users = session.CreateCriteria(typeof(User)).List<User>();
                return users;
            }
        }

        public User GetById(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                //事务(事务中的一系列事件，只要有一个不成功，之前成功的也会回滚，即插入成功的又被删除，修改成功的又恢复.....)
                //    transaction = session.BeginTransaction();//开启事务
                using (ITransaction transaction = session.BeginTransaction())
                {
                    User user = session.Get<User>(id);
                    transaction.Commit();

                    return user;
                }
            }
        }

        public User GetByUsername(string username)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                /*
                ICriteria criteria= session.CreateCriteria(typeof(User));
                criteria.Add(Restrictions.Eq("Username", username));//添加一个查询条件,第一个参数表示对哪个属性(字段）做限制，第二个表示值为多少
                User user = criteria.UniqueResult<User>();
                */
                User user = session.CreateCriteria(typeof(User)).Add(Restrictions.Eq("Account", username)).UniqueResult<User>();
                return user;

            }
        }

        /// <summary>
        /// NHibernate删除时根据主键更新，所以传来的对象user中得有主键
        /// </summary>
        /// <param name="user"></param>
        public void Remove(User user)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(user);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// NHibernate更新时根据主键更新，所以传来的对象user中得有主键
        /// </summary>
        /// <param name="user"></param>
        public void Update(User user)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Update(user);
                    transaction.Commit();
                }
            }
        }

        public bool VerifyUser(string username, string password)
        {

            using (ISession session = NHibernateHelper.OpenSession())
            {
                User user = session
                    .CreateCriteria(typeof(User))
                    .Add(Restrictions.Eq("Account", username))
                    .Add(Restrictions.Eq("Password", password))
                    .UniqueResult<User>();
                if (user == null) return false;
                return true;
            }
            
        }

        public User GetArticleContent(User account)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    User articleModel = session.Get<User>(account.Account);
                    transaction.Commit();
                    return articleModel;
                }
            }
        }

        public IList<User> GetUserList()
        {
            ISession session = NHibernateHelper.OpenSession();
            return session.QueryOver<User>().List();
        }
    }
}
