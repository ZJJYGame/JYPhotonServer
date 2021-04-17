using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public interface ISkillAdditionData
    {
        int DamgeAddition { get; }
        int CritProp { get; }
        int CritDamage { get; }
        int IgnoreDefensive { get; }
    }
}
