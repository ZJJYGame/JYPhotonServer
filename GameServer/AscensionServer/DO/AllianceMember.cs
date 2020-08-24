using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class AllianceMember : DataObject
    {
        public virtual int AllianceID { get; set; }
        public virtual string ApplyforMember { get; set; }
        public virtual string Member { get; set; }

        public AllianceMember()
        {
            ApplyforMember = null;
            Member = null;
        }


        public override void Clear()
        {
            AllianceID = -1;
            ApplyforMember = null;
            Member = null;
        }
    }
}
