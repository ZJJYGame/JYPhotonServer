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
/// 针对所有的工具
/// </summary>
namespace AscensionServer
{
    public partial class ServerBattleManager
    {

        /// <summary>
        /// 针对战斗中的随机数
        /// </summary>
        /// <param name="ov"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public int RandomManager(int ov,int minValue,int maxValue)
        {
            var targetValue = new Random((int)DateTime.Now.Ticks + ov).Next(minValue, maxValue);
            return targetValue;
        }
    }
}
