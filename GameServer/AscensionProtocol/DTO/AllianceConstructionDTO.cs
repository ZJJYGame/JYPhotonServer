using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
   public  class AllianceConstructionDTO:DataTransferObject
    {
        public virtual int AllianceID { get; set; }
        public virtual int AllianceCave { get; set; }
        public virtual int AllianceAlchemyStorage { get; set; }
        public virtual int AllianceScripturesPlatform { get; set; }
        public virtual int AllianceChamber { get; set; }

        public override void Clear()
        {
            AllianceID = -1;
            AllianceCave = 0;
            AllianceAlchemyStorage = 0;
            AllianceScripturesPlatform = 0;
            AllianceChamber = 0;
        }
    }
}
