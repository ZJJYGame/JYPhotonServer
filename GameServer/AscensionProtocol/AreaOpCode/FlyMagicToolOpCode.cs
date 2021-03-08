using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    /// <summary>
    /// 飞行法器操作码
    /// </summary>
   public enum FlyMagicToolOpCode
    {
        /// <summary>
        /// 获得新的飞行法器
        /// </summary>
        AddTool = 1,
        /// <summary>
        /// 获得飞行法器数据
        /// </summary>
        GetToolData=2,
        /// <summary>
        /// 更换飞行法器及人物属性
        /// </summary>
        UpdateStatus=3
    }
}
