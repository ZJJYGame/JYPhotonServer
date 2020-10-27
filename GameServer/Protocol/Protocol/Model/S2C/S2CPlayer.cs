using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
namespace Protocol
{
    [Serializable]
    [MessagePackObject]
    public class S2CPlayer: IDataContract
    {
        [Key(0)]
        public List<C2SPlayer> PlayerList { get; set; }
    }
}
