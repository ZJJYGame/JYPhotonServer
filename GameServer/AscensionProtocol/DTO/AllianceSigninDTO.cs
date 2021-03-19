using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class AllianceSigninDTO : DataTransferObject
    {
        public virtual int RoleID { get; set; }
        /// <summary>
        /// 宗门ID
        /// </summary>
        public virtual int AllianceID { get; set; }
        /// <summary>
        /// 宗门人气
        /// </summary>
        public virtual int Popularity { get; set; }
        /// <summary>
        /// 人物贡献
        /// </summary>
        public virtual int RoleContribution { get; set; }
        /// <summary>
        /// 宗门灵石
        /// </summary>
        public virtual int AllianceSpiritStone { get; set; }
        /// <summary>
        /// 是否签到
        /// </summary>
        public virtual bool IsSignin { get; set; }
        public override void Clear()
        {
            RoleID = 0;
            AllianceID = 0;
            Popularity = 0;
            RoleContribution = 0;
            AllianceSpiritStone = 0;
            IsSignin = false;
        }
    }
}
