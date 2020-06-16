using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionProtocol.BO
{
    public abstract class BusinessObject : IReference
    {
        public abstract void Clear();
    }
}
