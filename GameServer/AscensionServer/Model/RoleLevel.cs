using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Time :2020.4.28
 * Host : xianrenZhang
 * Content: 角色等级表
 * 
 */
namespace AscensionServer.Model
{
   public  class RoleLevel
    {
        public virtual int Id { get; set; }
        public virtual int RoleId { get; set; }
        public virtual byte RoleCurrentLevel { get; set; }
        public virtual int RoleExp { get; set; }

        public virtual Role Role_Id { get; set; }

    }
}
