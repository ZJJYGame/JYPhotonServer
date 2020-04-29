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

        IList<RoleGongFa> GetGongFaList();
        IList<Role> GetRoleList();
        IList<RoleLevel> GetRoleLevelList();

    }
}
