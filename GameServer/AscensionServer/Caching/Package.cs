using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    /// <summary>
    /// 消息包，需要设计成线程安全类型
    /// </summary>
    public class Package:IReference
    {
        /// <summary>
        /// 对应 ParameterCode
        /// </summary>
        public int PackageKey { get; set; }
        public string PackageDataJson { get; set; }
        public void Clear()
        {
            PackageKey = -1;
            PackageDataJson = null;
        }
    }
}
