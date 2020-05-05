/*
*Author : xianrenZhang
*Since 	:2020-04-28
*Description  : 角色管理类接口
*/
using AscensionServer.Model;
using System.Collections.Generic;

namespace AscensionServer.Manager
{
    public interface IRoleManager
    {
        Role AddRole(Role role);

        bool VerifyRole(string rolename);//角色名字

        ICollection<Role> GetAllRoles();

        Role GetByRolename(string rolename);


    }
}
