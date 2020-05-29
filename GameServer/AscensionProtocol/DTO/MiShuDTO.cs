using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class MiShuDTO:ProtocolDTO
    {
        public virtual int ID { get; set; }
        public virtual byte MiShuID { get; set; }
        public virtual int MiShuExp { get; set; }
        public virtual short MiShuLevel { get; set; }
        public virtual string MiShuSkillArry { get; set; }
    }
}