using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class ResourceUnitDTO:DataTransferObject
    {
        /// <summary>
        /// 此ID表示为生成时候分配的资源ID，而非全局ID
        /// </summary>
        public int ID{ get; set; }
        /// <summary>
        /// 等级；
        /// 若对象为生物，则表示为生物的等级；
        /// 若对象为材料，则此表示为品级
        /// 改为针对怪物浮动值
        /// </summary>
        public int FlowValue { get; set; }
        /// <summary>
        /// 位置信息
        /// </summary>
        public TransformDTO Position { get; set; }
        public int Amount { get; set; }
        /// <summary>
        /// 表示这个资源已经被占用
        /// </summary>
        public bool Occupied { get; set; }
        public override void Clear()
        {
            ID = -1;
            FlowValue = 0;
            Position?.Clear();
            Occupied = false;
        }
    }
}
