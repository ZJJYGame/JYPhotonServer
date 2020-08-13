using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class BattleInputC2S
    {
        /// <summary>
        /// 指令输入者ID
        /// </summary>
        public int CmdInputterID { get; set; }
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
        public int TargetID { get; set; }
    }
}
