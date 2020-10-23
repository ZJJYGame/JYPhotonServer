using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public  class AllianceSigninData
    {
        public int Role_Contribution { get; set; }
        public int Role_Reward { get; set; }
        public List<int> Role_Level { get; set; }
        public int Alliance_Spirit_Stone { get; set; }
        public int Alliance_Popularity { get; set; }
    }
}
