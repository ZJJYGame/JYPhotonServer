using Cosmos;
using System;
using System.Collections.Generic;

namespace AscensionServer
{
    /// <summary>
    /// 网络事件；
    /// 标准事件模型；
    /// key为short类型约束的枚举；
    /// value为object类型的对象
    /// </summary>
    public class OpCodeEventCore:ConcurrentStandardEventCore<ushort,object,OpCodeEventCore>
    {
    }
}
