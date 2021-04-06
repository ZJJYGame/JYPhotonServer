using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
   public enum SecondaryJobOpCode : byte
    {
        /// <summary>
        /// 获得炼丹数据
        /// </summary>
        GetSecondaryJobStatus = 1,
        /// <summary>
        /// 学习炼丹配方
        /// </summary>
        UpdateAlchemy=2,
        /// <summary>
        /// 开始炼丹
        /// </summary>
        CompoundAlchemy=3,  
        /// <summary>
        /// 学习傀儡配方
        /// </summary>
        UpdatePuppet=5,
        /// <summary>
        /// 炼制傀儡
        /// </summary>
        CompoundPuppet = 6,
        /// <summary>
        /// 学习锻造配方
        /// </summary>
        UpdateForge = 8,
        /// <summary>
        /// 开始锻造
        /// </summary>
        CompoundForge = 9,
        /// <summary>
        /// 组装傀儡
        /// </summary>
        AssemblePuppet=10
    }
}
