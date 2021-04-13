﻿using System.Collections;
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
    [MessagePackObject(true)]
    public class C2SMapResource 
    {
        /// <summary>
        /// 资源全局Id；
        /// </summary>
        public int ResId;
        /// <summary>
        /// 资源生成时候的索引；
        /// </summary>
        public int ResIndex;
        /// <summary>
        /// 资源所在的位置信息
        /// </summary>
        public FixVector3 ResPosition;
    }
}

















