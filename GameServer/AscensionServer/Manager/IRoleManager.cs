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
        void AddRole(User user);

        bool VerifyRole(string rolename, string gender);
      
    }
}
