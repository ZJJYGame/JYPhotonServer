using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 战斗房间内的输入指令
    /// </summary>
    [Serializable]
    public class RoomBattleInputC2S:IInputData
    {
        /// <summary>
        /// 房间ID
        /// </summary>
        public int RoomID { get; set; }
        /// <summary>
        /// 当前房间内的单个指令
        /// </summary>
        public BattleInputData BattleInputC2S { get; set; }
        public ushort InputCmdType { get; set; }
    }
}
