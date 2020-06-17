using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionProtocol.VO
{
    public abstract class ViewObject : IReference
    {
        public abstract void Clear();
    }
}
