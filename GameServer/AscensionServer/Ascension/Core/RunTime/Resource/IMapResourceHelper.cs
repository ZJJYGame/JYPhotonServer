using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public interface IMapResourceHelper
    {
        void LoadMapResource();
        void TakeUpMapResource(Dictionary<byte,object> dataMessage);
        void OnRoleEnterMap(RoleEntity role);
        void OnRefreshResource();
    }
}


