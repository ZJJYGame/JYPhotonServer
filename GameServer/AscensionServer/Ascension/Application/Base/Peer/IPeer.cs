using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    public interface IPeer: IReference
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        uint Conv { get; }
        /// <summary>
        /// 是否存活；
        /// </summary>
        bool Available { get; }
        /// <summary>
        /// peer对象Handle
        /// </summary>
        object Handle { get; }
    }
}
