using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionHive
{
    [Serializable]
    public abstract class AscensionEventBase
    {

        public byte Code { get; set; }
    }
}
