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
            recordHelper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IRecordHelper>();
            if(recordHelper==null)
                Utility.Debug.LogError($"{this.GetType()} has no helper instance ,base type: {typeof(IRecordHelper)}");
        }
        public void RecordRole(int roleId,object data)
        {
            recordHelper.RecordRole(roleId,data);
        }
        int updateInterval = ApplicationBuilder._MSPerTick;
        long latestTime;
        int roomIndex;
        public override void OnRefresh()
        {
            var now = Utility.Time.MillisecondTimeStamp();
            if (now >= latestTime)
            {
                //广播当前帧，并进入下一帧；
                latestTime = now + updateInterval;
                Utility.Debug.LogWarning("RecordManager OnRefresh");
            }
        }
    }
}
