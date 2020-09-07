using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public interface IRole:ISimpleKeyValue<byte,object>
    {
        /// <summary>
        /// roleId，由peer缓存
        /// </summary>
        uint RoleId { get; }
        byte DataCount { get; }
    }
}
