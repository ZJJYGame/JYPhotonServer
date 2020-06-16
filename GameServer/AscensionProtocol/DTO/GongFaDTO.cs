using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class GongFaDTO: DataTransferObject
    {
        public virtual int ID { get; set; }
        public virtual byte GongFaID { get; set; }
        public virtual int GongFaExp { get; set; }
        public virtual short GongFaLevel { get; set; }
        public virtual string GongFaSkillArray { get; set; }

        public override void Clear()
        {
            ID =-1;
            GongFaID = 0;
            GongFaExp = 0;
            GongFaLevel = 0;
            GongFaSkillArray = null;
        }
    }
}
