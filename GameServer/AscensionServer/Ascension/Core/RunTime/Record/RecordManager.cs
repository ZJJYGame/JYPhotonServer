using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Cosmos;
namespace AscensionServer
{
    [CustomeModule]
    public class RecordManager:Module<RecordManager>
    {
        IRecordHelper recordHelper;
        public override void OnInitialization()
        {
        }
        public void RecordTime(long sessionId)
        {

        }
    }
}
