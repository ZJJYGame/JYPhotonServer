using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    /// <summary>
    /// 用于角色技能升级记录传输数据的类
    /// </summary>
    [Serializable]
  public  class RoleAllianceSkilltransferDTO:DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual int RoleAssets { get; set; }
        public virtual int Contribution { get; set; }
        public virtual int SkillStrong { get; set; }
        public virtual int SkillInsight { get; set; }
        public virtual int SkillMeditation { get; set; }
        public virtual int SkillRapid { get; set; }

        public override void Clear()
        {
            RoleID = 0;
            RoleAssets = 0;
            Contribution = 0;
            SkillStrong = 0;
            SkillInsight = 0;
            SkillMeditation = 0;
            SkillRapid = 0;

        }
    }
}
