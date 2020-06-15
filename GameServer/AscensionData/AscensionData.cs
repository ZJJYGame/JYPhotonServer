using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer;
namespace AscensionData
{
    public class AscensionData : IDisposable
    {
        public int Index { get; set; }

        public void Clear()
        {
            Index = -1;
        }

        public void Dispose()
        {
            Index = -1;
        }
    }
}
