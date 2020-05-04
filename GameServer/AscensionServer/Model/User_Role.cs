using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*关联uuid外键*/
namespace AscensionServer.Model
{
    public class User_Role
    {
        public virtual string UUID { get; set; }
        public virtual string Role_Id_Array { get; set; }

        public virtual void AddStr(User_Role str)
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

        public virtual void UpdateStr(User_Role str)
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

        public virtual string GetArray(string uuid)
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
