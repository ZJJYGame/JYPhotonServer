using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class MiShu:ProtocolDTO
    {
        public virtual int ID { get; set; }
        public virtual string SkillArry { get; set; }
        public virtual int MiShuId { get; set; }
        public virtual int MiShuExp { get; set; }
        public virtual int MiShuLevel { get; set; }

        public override void Clear()
        {
            ID = 0;
            SkillArry = null;
            MiShuId = 0;
            MiShuExp = 0;
            MiShuLevel = 0;
        }
    }
}