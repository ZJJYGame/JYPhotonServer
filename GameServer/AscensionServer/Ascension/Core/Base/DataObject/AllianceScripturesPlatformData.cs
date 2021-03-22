using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public  class AllianceScripturesPlatformData
    {
        public int ItemID { get; set; }
        public int ContributionDown { get; set; }
        public int ContributionUp { get; set; }
        
    }
}
