using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MessagePack;
namespace Protocol
{
    /// <summary>
    /// 表示一个单位；
    /// 历练&野外资源数据；
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class FixMapRes
    {
        [Key(0)]
        public int ResId;
        [Key(1)]
        public int ResAmount;
        [Key(2)]
        public FixVector3 ResSpawnPositon;
        [Key(3)]
        public int ResSpawnRange;
    }
}
