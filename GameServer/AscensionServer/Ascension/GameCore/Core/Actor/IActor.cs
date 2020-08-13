using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AscensionServer
{
    public  interface IActor
    {
        /// <summary>
        /// Actor类型，AI还是玩家peer
        /// </summary>
        byte ActorType { get;  }
    }
}
