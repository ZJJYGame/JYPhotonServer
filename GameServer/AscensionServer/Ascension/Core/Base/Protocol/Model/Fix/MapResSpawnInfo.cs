using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MessagePack;
namespace AscensionServer
{
    /// <summary>
    /// 地图资源生成信息，不作传输使用；
    /// </summary>
    [Serializable]
    public class MapResSpawnInfo
    {
        public int ResId { get; set; }
        public int ResAmount { get; set; }
        public FixVector3 ResSpawnPositon { get; set; }
        public int ResSpawnRange { get; set; }
        public LevelResType ResType { get; set; }
    }
}


















