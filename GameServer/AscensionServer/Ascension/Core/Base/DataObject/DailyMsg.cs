using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class DailyMsg
    {
        public int MsgIndex { get; set; }
        public string MsgType { get; set; }
        public string MsgContent { get; set; }
    }
}
