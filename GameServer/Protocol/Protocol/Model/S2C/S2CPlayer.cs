using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
namespace Protocol
{
    [Serializable]
    [MessagePackObject]
    public class SessionRoleIds: IDataContract
    {
        [Key(0)]
        public List<SessionRoleIdPair> SessionRoleIdList { get; set; }
    }
}
