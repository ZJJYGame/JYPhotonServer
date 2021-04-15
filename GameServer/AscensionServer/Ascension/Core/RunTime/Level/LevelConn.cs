using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
namespace AscensionServer
{
    [MessagePackObject(true)]
    public class LevelConn:IDisposable
    {
        public int RoleId { get; set; }
        public byte LevelType { get; set; }
        public int LevelId { get; set; }
        public long ServerTick { get; set; }
        public long ClientTick { get; set; }
        public RoleEntity RoleEntity { get; set; }
        public FixTransportData InputData{ get; set; }

        public void Dispose()
        {
            RoleId = -1;
            ServerTick= 0;
            ClientTick= 0;
            RoleEntity= null;
            LevelType = 0;
            LevelId = 0;
            InputData = null;
        }
    }
}
