using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;

namespace AscensionServer
{
    /// <summary>
    /// 用于初始化 战斗数据 和管理 战斗的数据缓存
    /// </summary>
    public partial class ServerBattleManager
    {
        /// <summary>
        /// 初始化 DTO  对应的房间id
        /// </summary>
        public Dictionary<int, BattleInitDTO> _teamIdToBattleInit = new Dictionary<int, BattleInitDTO>();
        /// <summary>
        /// 房间id， 每回合战斗传输数据对象
        /// </summary>
        public Dictionary<int, List<BattleTransferDTO>> _roomidToBattleTransfer = new Dictionary<int, List<BattleTransferDTO>>();
        /// <summary>
        /// 回收房间
        /// </summary>
        public List<int> _oldBattleList = new List<int>();
        /// <summary>
        /// 房间id
        /// </summary>
        int _roomId = 1000;





    }
}
