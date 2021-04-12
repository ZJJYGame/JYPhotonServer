using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    [Serializable]
    [MessagePackObject]
    public class C2SContainer: IDataContract
    {
        [Key(0)]
        public FixContainer Container { get; set; }
        [Key(1)]
        public SessionRoleIdPair Player { get; set; }
    }
}
