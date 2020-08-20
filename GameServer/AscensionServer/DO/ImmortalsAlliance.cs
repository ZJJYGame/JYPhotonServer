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
        public virtual int ID { get; set; }
        public virtual int ImmortalsallianceLevel { get; set; }
        public virtual int immortalsallianceNumberPeople { get; set; }
        public virtual int ImmortalsalliancePeopleMax { get; set; }
        public virtual string ImmortalsallianceMaster { get; set; }
        public virtual string ImmortalsallianceName { get; set; }
        public virtual bool IsApplyfor { get; set; }

        public override void Clear()
        {
            ID = -1;
            ImmortalsallianceLevel = 0;
            immortalsallianceNumberPeople = 0;
            ImmortalsalliancePeopleMax = 0;
            ImmortalsallianceMaster = null;
            ImmortalsallianceName = null;
            IsApplyfor = false;
        }
        public ImmortalsAlliance()
        {
            ImmortalsallianceLevel = 1;
            immortalsallianceNumberPeople = 1;
            ImmortalsalliancePeopleMax =100;
            ImmortalsallianceMaster = null;
            ImmortalsallianceName = null;
            IsApplyfor = false;

        }

    }
}
