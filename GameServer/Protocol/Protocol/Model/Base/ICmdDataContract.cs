using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace Protocol
{
    [Union(0, typeof(CmdInput))]
    public interface ICmdDataContract: IDataContract
    {
        byte CmdKey { get; }
    }
}
