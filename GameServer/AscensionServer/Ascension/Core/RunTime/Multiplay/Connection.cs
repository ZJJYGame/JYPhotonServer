using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionServer
{
    public class Connection
    {
        public int Conv { get; set; }
        public Dictionary<long, string> FrameDict { get; private set; }
        public Connection()
        {
            FrameDict = new Dictionary<long, string>();
        }
    }
}
