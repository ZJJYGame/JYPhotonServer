using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionProtocol.AO
{
    public abstract class ApplicationObject : IReference
    {
        public abstract void Clear();
    }
}
