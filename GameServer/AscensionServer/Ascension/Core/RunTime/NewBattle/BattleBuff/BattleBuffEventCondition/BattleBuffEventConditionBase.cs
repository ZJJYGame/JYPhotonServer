using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace AscensionServer
{
    public class BattleBuffEventConditionBase
    {
        public bool CanTrigger()
        {
            int value = Utility.Algorithm.CreateRandomInt(0, 100);
            if (value < 50)
                return true;
            else return false;
        }
    }
}
