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
    /// 表示一个单位；
    /// 历练&野外资源数据；
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public class FixMapResource
    {
        public int ResId { get; set; }
        public int ResAmount { get; set; }
        public FixVector3 ResSpawnPositon { get; set; }
        public int ResSpawnRange { get; set; }
    }
}


















