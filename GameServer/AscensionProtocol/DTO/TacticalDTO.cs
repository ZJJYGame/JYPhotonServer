using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class TacticalDTO : DataTransferObject
    {
        /// <summary>
        ///阵法释放者
        /// </summary>
        public int RoleID { get; set; }
        /// <summary>
        /// 此ID表示为添加时候分配的资源ID，而非全局ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 阵法的全局ID
        /// </summary>
        public int GlobalID { get; set; }
        /// <summary>
        /// 阵法召唤的怪物ID
        /// </summary>
        public int MonsterID { get; set; }
        /// <summary>
        /// 阵法的位置信息
        /// </summary>
        public TransformDTO transformDTO { get; set; }
        /// <summary>
        /// 阵法覆盖范围
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// 阵法的隐匿值
        /// </summary>
        public int Dormant { get; set; }

        public override void Clear()
        {
            ID = -1;
            RoleID = 0;
            GlobalID = 0;
            MonsterID = 0;
            transformDTO?.Clear();
            Duration = 0;
            Dormant = 0;
        }
    }
}
