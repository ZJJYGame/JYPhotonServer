using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
namespace Protocol
{
    /// <summary>
    /// 飞行法器输入；
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class C2SFlyMagicToolInput:IDataContract
    {
        [Key(0)]
        public int MagicToolId { get; set; }
    }
}
