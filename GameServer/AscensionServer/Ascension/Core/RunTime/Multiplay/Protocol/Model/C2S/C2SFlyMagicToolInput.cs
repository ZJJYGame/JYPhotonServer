using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
namespace AscensionServer
{
    /// <summary>
    /// 飞行法器输入；
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public class C2SFlyMagicToolInput:IDataContract
    {
        public int MagicToolId { get; set; }
    }
}
