using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/* Time  2020.4.28
 * Content : 角色表
 * Host : xianrenZhang 
 */
namespace AscensionServer.Model
{
    public class Role
    {
        public virtual int RoleId { get; set; }
        public virtual string RoleName { get; set; }
        public virtual string RoleRoot { get; set; }
        public virtual int RoleLevel{ get; set; }
        public virtual int RoleExp { get; set; }
        //public virtual IList<User> Users { get; set; }
    }
}
