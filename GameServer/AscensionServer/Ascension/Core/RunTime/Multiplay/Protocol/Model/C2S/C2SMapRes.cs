using System.Collections;
using System.Collections.Generic;
using MessagePack;
using System;
using UnityEngine;

namespace AscensionServer
{
    /// <summary>
    /// 野外&历练资源单位
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class C2SMapRes: IDataContract
    {
        /// <summary>
        /// 资源全局Id；
        /// </summary>
        [Key(0)]
        public int ResId;
        /// <summary>
        /// 资源生成时候的索引；
        /// </summary>
        [Key(1)]
        public int ResIndex;
        /// <summary>
        /// 资源所在的位置信息
        /// </summary>
        [Key(2)]
        public FixVector3 ResPosition;
    }
}