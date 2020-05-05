using AscensionServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public interface IUser_RoleManager
    {
        void AddStr(User_Role str);
        void UpdateStr(User_Role str);
        string GetArray(string uuid);
    }
}
