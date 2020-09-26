using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    public class MessageChannel
    {
        /// <summary>
        /// 默认频道
        /// </summary>
        public const byte DEFAULT_CHANNEL = 0;
        /// <summary>
        /// 战斗频道
        /// </summary>
        public const byte BATTLE_CHANNEL = 1;
        /// <summary>
        /// 探索频道
        /// </summary>
        public const byte ADVENTURE_CHANNEL = 2;
    }
}
