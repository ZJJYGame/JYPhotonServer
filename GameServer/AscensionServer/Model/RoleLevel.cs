/*
*Author : xianrenZhang
*Since :	2020-04-28
*Description :  角色等级表
*/
using System;
namespace AscensionServer.Model
{
    [Serializable]
    public  class RoleLevel
    {
        public virtual int Id { get; set; }
        public virtual int RoleId { get; set; }
        public virtual byte RoleCurrentLevel { get; set; }
        public virtual int RoleExp { get; set; }

        public virtual Role Role_Id { get; set; }

    }
}
