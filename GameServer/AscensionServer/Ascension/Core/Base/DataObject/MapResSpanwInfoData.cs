using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 历练&秘境资源生成信息字典；
    /// 不用作传输；
    /// </summary>
    [Serializable]
    public class MapResSpanwInfoData : Data
    {
        /// <summary>
        /// 此数据为：地图资源的生成信息。将数据转换Fix类型后用于传输；
        /// </summary>
        public Dictionary<int, MapResSpawnInfo> MapResSpawnInfoDict { get; set; }
        public override void SetData(object data)
        {
            MapResSpawnInfoDict = data as Dictionary<int, MapResSpawnInfo>;
        }
    }
}


