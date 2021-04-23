using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RoleGongFaDTO : DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual Dictionary<int, CultivationMethodDTO> GongFaIDDict { get; set; }
        public RoleGongFaDTO()
            {
            GongFaIDDict = new Dictionary<int, CultivationMethodDTO>();
        }
        public override void Clear()
        {
            RoleID = -1;
            GongFaIDDict = null;
        }
    }
    [Serializable]
    public class CultivationMethodDTO : DataTransferObject
    {
        public virtual int CultivationMethodID { get; set; }
        public virtual int CultivationMethodExp { get; set; }
        public virtual short CultivationMethodLevel { get; set; }
        public virtual List<int> CultivationMethodLevelSkillArray { get; set; }
        public CultivationMethodDTO()
        {
            CultivationMethodLevelSkillArray = new List<int>();
        }
        public override void Clear()
        {
            CultivationMethodID = 0;
            CultivationMethodExp = 0;
            CultivationMethodLevel = 0;
            CultivationMethodLevelSkillArray.Clear();
        }
    }
}
