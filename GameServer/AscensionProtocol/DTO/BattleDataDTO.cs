using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    public class BattleDataDTO:ProtocolDTO
    {
        public int TargetID { get; set; }
        public int InstructionID { get; set; }
        public override void Clear()
        {
            TargetID = 0;
            InstructionID = 0;
        }
    }
}
