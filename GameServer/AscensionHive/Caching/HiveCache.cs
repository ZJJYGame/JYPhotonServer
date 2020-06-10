using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionHive
{
    public abstract class HiveCache : IHive
    {
        public abstract void Dispose();
    }
}
