using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    /// <summary>
    /// 人物资产操作指令
    /// </summary>
   public enum RoleAssetsOpCode
    {
        /// <summary>
        /// 增加资产
        /// </summary>
        AddAssets = 1,
        /// <summary>
        /// 获得资产
        /// </summary>
        GetAssets=2,
        /// <summary>
        /// 消耗资产
        /// </summary>
        ReduceAssets=3
    }
}
