using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class AllianceConstruction : DataObject
    {
        public virtual int AllianceID { get; set; }
        public virtual int AllianceCave { get; set; }
        public virtual int AllianceAlchemyStorage { get; set; }
        public virtual int AllianceScripturesPlatform { get; set; }
        public virtual int AllianceChamber { get; set; }
        public virtual int AllianceAssets { get; set; }

        public AllianceConstruction()
        {
            AllianceID = -1;
            AllianceCave = 1;
            AllianceAlchemyStorage = 1;
            AllianceScripturesPlatform = 1;
            AllianceChamber = 1;
            AllianceAssets = 1000000;
        }
        public override void Clear()
        {
            AllianceID = -1;
            AllianceCave = 0;
            AllianceAlchemyStorage = 0;
            AllianceScripturesPlatform = 0;
            AllianceChamber = 0;
            AllianceAssets = 0;
        }
    }
}
