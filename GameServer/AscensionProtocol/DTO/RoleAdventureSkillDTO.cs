using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    public class RoleAdventureSkillDTO : DataTransferObject
    {
        public int RoleID { get; set; }
        public bool IsInUse { get; set; }
        public int SkillID  { get; set; }
        public int CDInterval { get; set; }
        public int BuffeInterval { get; set; }
        public FeatureSkillTypeEnum featureSkillTypeEnum { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            IsInUse = false;
            SkillID = 0;
            CDInterval = 0;
            BuffeInterval = 0;
        }
        /// <summary>
        /// 无技能使用
        /// 远遁
        /// 加速移动
        /// 隐匿
        /// 查看周围隐匿单位
        /// 破除隐匿
        /// 闪现
        /// </summary>
        public enum FeatureSkillTypeEnum : short
        {
            Zero = 0,
            Tp = 2230,
            SpeetMove = 2231,
            Stealth = 2232,
            Visible = 2233,
            Eradicate = 2234,
            FlashBeforeOne = 2235
            //    0,
            //2230,
            //2231,
            //2232,
            //2233,
            //2234
        }
    }
}
