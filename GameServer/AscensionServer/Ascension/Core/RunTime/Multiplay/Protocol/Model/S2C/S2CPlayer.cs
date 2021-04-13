using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
namespace AscensionServer
{
    [Serializable]
    [MessagePackObject(true)]
    public class SessionRoleIds
    {
        public List<SessionRoleIdPair> SessionRoleIdList { get; set; }
    }
}
