using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
namespace Protocol
{
    [Serializable]
    [MessagePackObject(true)]
    public class SessionRoleIds: IDataContract
    {
        public List<SessionRoleIdPair> SessionRoleIdList { get; set; }
    }
}
