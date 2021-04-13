using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
namespace AscensionServer
{
    [Serializable]
    [MessagePackObject(true)]
    public class S2CMapResource 
    {
        public FixContainer Container { get; set; }
        /// <summary>
        /// 一组C2SWildResource的list集合；
        /// </summary>
        public List<C2SMapResource> MapResourceList { get; set; }
    }
}
