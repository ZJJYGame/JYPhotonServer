using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class RoleAlliance : DataObject
    {
        public virtual int RoleID { get; set; }
        public virtual int AllianceID { get; set; }
        public virtual byte AllianceJob { get; set; }
        public virtual int Reputation { get; set; }
        public virtual int ReputationMonth { get; set; }
        public virtual int ReputationHistroy { get; set; }
        public virtual string JoinTime { get; set; }
        public virtual string JoinOffline { get; set; }
        public virtual string RoleName { get; set; }
        public virtual string ApplyForAlliance { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            AllianceID = 0;
            AllianceJob = 0;
            Reputation = 0;
            ReputationMonth = 0;
            ReputationHistroy = 0;
            JoinTime = null;
            JoinOffline = null;
            ApplyForAlliance = null;
        }
        public RoleAlliance()
        {
            RoleID = -1;
            AllianceID = 0;
            AllianceJob = 0;
            Reputation = 0;
            ReputationMonth = 0;
            ReputationHistroy = 0;
            JoinTime = null;
            JoinOffline = null;
            ApplyForAlliance = null;
        }

    }
}
