/*
*Author : xianrenZhang
*Since :	2020-04-28
*Description :  用户名
*/

using System.Collections.Generic;

namespace AscensionServer.Model
{
    public class User
    {
        public virtual string UUID { get; set; }
        public virtual string Account { get; set; }
        public virtual string Password { get; set; }
        //public virtual ISet<UserRole> UserroleList { get; set; }
        //public virtual IList<Role> Roles { get; set; }
    }

}
