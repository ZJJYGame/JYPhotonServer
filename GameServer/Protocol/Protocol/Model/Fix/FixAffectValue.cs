//***********************************************************
// 描述：
// 作者：Don  
// 创建时间：2020-11-04 16:13:28
// 版 本：1.0
//***********************************************************
using Cosmos;
using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Protocol
{
    //[MessagePackObject]
    [Serializable]
    /// <summary>
    /// 影响类型；
    /// </summary>
    public class FixAffectValue
    {
        [Key(0)]
        /// <summary>
        /// 消耗类型
        /// </summary>
        public byte AffectType;
        [Key(1)]
        /// <summary>
        /// 固定数值；
        /// </summary>
        public int AffectValue;
        [Key(2)]
        /// <summary>
        /// 百分比；
        /// </summary>
        public int AffectPercent;
        [Key(3)]
        /// <summary>
        /// 间隔；
        /// 1s写作1000；
        /// </summary>
        public int AffectInterval;
    }
}