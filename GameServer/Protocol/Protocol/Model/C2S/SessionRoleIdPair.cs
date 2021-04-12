using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
using Cosmos;
namespace Protocol
{
    [Serializable]
    [MessagePackObject]
    public class SessionRoleIdPair :  IDataContract, IDisposable
    {
        [Key(0)]
        public int SessionId { get; set; }
        [Key(1)]
        public int RoleId { get; set; }
        public SessionRoleIdPair() { }
        public SessionRoleIdPair(int sessionId, int playerId)
        {
            SessionId = sessionId;
            RoleId = playerId;
        }
        public void Dispose()
        {
            SessionId = 0;
            RoleId = 0;
        }
        public override string ToString()
        {
            return $"SessionId:{SessionId} ; RoleId:{RoleId}";
        }
    }
}
