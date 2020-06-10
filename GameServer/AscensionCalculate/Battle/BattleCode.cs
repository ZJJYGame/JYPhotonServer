using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionCalculator
{
    //================================================
    //战斗中的操作码，约束为byte。
    //
    //================================================
    public enum BattleCode:byte
    {
        ChooseEnemy=0,
        ChooseTeammate = 1,
    }
}
