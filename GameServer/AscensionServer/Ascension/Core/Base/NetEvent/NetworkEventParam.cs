using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AscensionServer
{
    public class NetworkEventParam
    {
        /// <summary>
        /// 队伍吗，命令
        /// </summary>
        public const ushort  TEAM_CMD= 0;
        /// <summary>
        /// 战斗命令
        /// </summary>
        public const ushort  BATTLE_CMD= 1;
        /// <summary>
        /// peer断线命令
        /// </summary>
        public const ushort  PEER_DISCONNECT= 2;
    }
}
