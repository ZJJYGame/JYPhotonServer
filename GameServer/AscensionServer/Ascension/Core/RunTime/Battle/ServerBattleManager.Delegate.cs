using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using NHibernate.Linq.Clauses;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// 存放战斗所有的委托
/// </summary>
namespace AscensionServer
{
    /// <summary>
    ///针对 每回合战斗倒计时结束 回调方法 
    /// </summary>
    public delegate void BattleEndDelegateHandle();
    /// <summary>
    /// 针对  战斗准备倒计时结束  
    /// </summary>
    public delegate void BattlePrepareDelegateHandle();

    /// <summary>
    /// 针对  组队 战斗开始的时候收集
    /// </summary>
    public delegate void BattleStartDelegateHandle();

}
