using System.Collections;
using System.Collections.Generic;
using Cosmos;
using Protocol;

namespace  AscensionServer{
    /// <summary>
    /// 指令事件EventCore；
    /// byte表示 ProtocolDefine.OPERATION中的操作码
    /// </summary>
    public class CommandEventCore :ConcurrentEventCore<ushort, OperationData, CommandEventCore>
    {
        
    }
}