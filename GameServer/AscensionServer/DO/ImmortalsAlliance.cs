using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class ImmortalsAlliance : DataObject
    {
        public virtual int RoleID { get; set; }
        public virtual string allianceStatus { get; set; }
        public virtual bool IsMaster { get; set; }
        public virtual string AllianceManifesto { get; set; }
        public virtual string AllianceName { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            allianceStatus = null;
            IsMaster = false;
            AllianceManifesto = null;
            AllianceName = null;
        }
        public ImmortalsAlliance()
        {
            allianceStatus = null;
            IsMaster = false;
            AllianceManifesto = null;
            AllianceName = null;
        }

    }
}
