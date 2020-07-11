using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    /// <summary>
    /// MonsterTransformQueueDTO的信息队列
    /// </summary>
    /// 
    [Serializable]
    public class MonsterTransformQueueDTO : DataTransferObject
    {
        public int MonsterGlobal { get; set; }
        public int MonsterID { get; set; }
        public Queue<TransformDTO> TransformSet { get; set; }
        public MonsterTransformQueueDTO()
        {
            MonsterGlobal = -1;
            MonsterID = -1;
            TransformSet = new Queue<TransformDTO>();
        }
        public override void Clear()
        {
            MonsterGlobal = -1;
            MonsterID = -1;
            TransformSet.Clear();
        }
    }
}
