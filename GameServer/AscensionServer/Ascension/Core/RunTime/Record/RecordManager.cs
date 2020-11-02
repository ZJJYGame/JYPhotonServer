using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Cosmos;
using Protocol;

namespace AscensionServer
{
    [CustomeModule]
    public class RecordManager : Module<RecordManager>
    {
        IRecordHelper recordHelper;
        public override void OnInitialization()
        {
            recordHelper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IRecordHelper>();
            if (recordHelper == null)
                Utility.Debug.LogError($"{this.GetType()} has no helper instance ,base type: {typeof(IRecordHelper)}");
        }
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPR_PLYAER_LOGOFF,OnPlayerLogoff);
        }
        public void RecordRole(RoleEntity roleEntity)
        {
            recordHelper.RecordRole(roleEntity);
        }
        void OnPlayerLogoff(OperationData opData)
        {
            var roleEntity = opData.DataMessage as RoleEntity;
            recordHelper.RecordRole(roleEntity);
        }
    }
}