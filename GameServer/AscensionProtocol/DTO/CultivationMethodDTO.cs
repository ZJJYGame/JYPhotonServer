using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class CultivationMethodDTO: DataTransferObject
    {
        public virtual int ID { get; set; }
        public virtual int CultivationMethodID { get; set; }
        public virtual int CultivationMethodExp { get; set; }
        public virtual short CultivationMethodLevel { get; set; }
        public virtual List<int> CultivationMethodLevelSkillArray { get; set; }
        public override void Clear()
        {
            ID =-1;
            CultivationMethodID = 0;
            CultivationMethodExp = 0;
            CultivationMethodLevel = 0;
            CultivationMethodLevelSkillArray.Clear();
        }
    }
}
