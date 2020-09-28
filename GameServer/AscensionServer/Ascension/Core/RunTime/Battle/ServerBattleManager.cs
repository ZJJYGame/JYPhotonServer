using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using Cosmos;
namespace AscensionServer
{
    /// <summary>
    /// 没统一的服务器战斗功能
    /// </summary>
    [CustomeModule]
    public class ServerBattleManager : Module<ServerBattleManager>
    {
        //public Dictionary<int,int> 
        /*
         * 
         * 3.
         * 2.加载resource json数据表
         * 1.需要怎么存，怎么管理*/

        /// <summary>
        /// 队伍id， 战斗初始化对象
        /// </summary>
        Dictionary<int, BattleInitDTO> _teamIdToBattleInit = new Dictionary<int, BattleInitDTO>();
        /// <summary>
        /// 房间id， 战斗传输数据对象
        /// </summary>
        Dictionary<int, List<BattleTransferDTO>> _roomidToBattleTransfer = new Dictionary<int, List<BattleTransferDTO>>();
        /// <summary>
        /// 回收房间
        /// </summary>
        public List<int> _oldBattleList = new List<int>();
        /// <summary>
        /// 房间id
        /// </summary>
        int _roomId = 1000;


        /// <summary>
        /// 进入战斗
        /// </summary>
        public void EntryBattle(BattleInitDTO battleInitDTO)
        {
            BattleInitDTO battleInit;
            if (_oldBattleList.Count > 0 )
            {
                int roomid = _oldBattleList[0];
                battleInit = _teamIdToBattleInit[roomid];
            }
        }

    }
}
