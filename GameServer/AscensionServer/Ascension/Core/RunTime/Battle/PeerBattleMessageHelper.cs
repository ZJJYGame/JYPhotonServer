using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class PeerBattleMessageHelper : IBattleMessageHelper
    {
        public void BattleMessageHandler(object data)
        {
            BattleInputData bid = data as BattleInputData;
        }
    }
}
