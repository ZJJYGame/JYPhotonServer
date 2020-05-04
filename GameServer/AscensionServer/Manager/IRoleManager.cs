using AscensionServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
