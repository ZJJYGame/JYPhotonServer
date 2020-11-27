using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
namespace Protocol
{
    [Serializable]
    [MessagePackObject]
    public class S2CFieldResource : IDataContract
    {
        [Key(0)]
        public FixContainer Container { get; set; }
        /// <summary>
        /// 一组C2SWildResource的list集合；
        /// </summary>
        [Key(1)]
        public List<C2SFieldResource> WildResourceList { get; set; }
    }
}
