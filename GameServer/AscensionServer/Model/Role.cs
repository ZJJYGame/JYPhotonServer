/*
*Author : xianrenZhang
*Since :	2020-04-28
*Description :  角色表
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class Role
    {
        public virtual int RoleId { get; set; }
        public virtual string RoleName { get; set; }
        public virtual string RoleRoot { get; set; }
        public virtual int RoleLevel{ get; set; }
        public virtual int RoleExp { get; set; }
        public virtual byte RoleGender { get; set; }
    }
}
