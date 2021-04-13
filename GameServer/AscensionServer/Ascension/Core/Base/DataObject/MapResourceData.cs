using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 野外历练资源数据集对象
    /// </summary>
    [Serializable]
    public class MapResourceData : Data
    {
        public Dictionary<int, FixMapResource> FixFieldResourceDict { get; set; }
        public override void SetData(object data)
        {
            //FixFieldResourceDict = data as Dictionary<int, FixMapRes>;
        }
    }
}


