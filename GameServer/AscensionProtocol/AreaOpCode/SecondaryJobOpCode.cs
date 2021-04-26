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
        /// 学习副职业配方
        /// </summary>
        StudySecondaryJobStatus = 2,
        /// <summary>
        /// 开始炼丹
        /// </summary>
        CompoundAlchemy=3,
        /// <summary>
        /// 合成阵法
        /// </summary>
        CompoundTactic = 4,
        /// <summary>
        /// 合成符箓
        /// </summary>
        CompoundRunes = 5,
        /// <summary>
        /// 炼制傀儡
        /// </summary>
        CompoundPuppet = 6,
        /// <summary>
        /// 修复傀儡
        /// </summary>
        RepairPuppet=7,
        /// <summary>
        /// 开始锻造
        /// </summary>
        CompoundForge = 9,
        /// <summary>
        /// 组装傀儡
        /// </summary>
        AssemblePuppet=10,
        /// <summary>
        /// 获得傀儡组件
        /// </summary>
        GetPuppetUnit = 11,
        /// <summary>
        /// 添加妖灵精魄
        /// </summary>
        AddDemonicSoul=12,
        /// <summary>
        /// 合成妖灵精魄
        /// </summary>
        CompoundDemonicSoul = 13,
        /// <summary>
        /// 获得妖灵精魄
        /// </summary>
        GetDemonicSoul = 14,

    }
}
