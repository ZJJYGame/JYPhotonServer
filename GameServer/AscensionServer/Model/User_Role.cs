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

       
    }
}
