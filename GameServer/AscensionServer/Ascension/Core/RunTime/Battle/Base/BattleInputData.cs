using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 战斗输入指令对象
    /// </summary>
    [Serializable]
    public class BattleInputData:IInputData
    {
        /// <summary>
        /// 指令输入的类型
        /// 移动、使用道具、攻击、技能、添加好友等
        /// 参考 OperationCode的类型;
        /// </summary>
        public ushort InputCmdType { get; set; }
        /// <summary>
        /// 指令输入者类型;
        /// 0：玩家；
        /// 1：宠物；
        /// 2：AI类型01
        /// 3：AI类型02
        ///      etc . . . 
        /// </summary>
        public byte CmdInputterType { get; set; }
        /// <summary>
        /// 指令输入者ID
        /// </summary>
        public uint CmdInputterID { get; set; }
        /// <summary>
        /// 指令ID；
        /// 例如使用功法秘术技能，或使用道具是不同的指令；
        /// </summary>
        public int CmdID{ get; set; }
        /// <summary>
        /// 指令字段ID；
        /// 当前指令下，例如使用了道具的具体ID，技能的具体ID；
        /// </summary>
        public int CmdFieldID{ get; set; }
        /// <summary>
        /// 目标ID
        /// </summary>
        public uint TargetID { get; set; }

    }
}
