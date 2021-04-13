using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Cosmos;

namespace AscensionServer
{
    [Module]
    public class RecordManager :Cosmos. Module, IRecordManager
    {
        IRecordHelper recordHelper;
        public override void OnActive()
        {
            recordHelper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IRecordHelper>();
            if (recordHelper == null)
                Utility.Debug.LogError($"{this.GetType()} has no helper instance ,base type: {typeof(IRecordHelper)}");
        }
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.LogoffRole, OnPlayerLogoff);
        }
        public void RecordRole(RoleEntity roleEntity)
        {
            recordHelper.RecordRole(roleEntity);
        }
        void OnPlayerLogoff(int sessionId, OperationData opData)
        {
            var roleEntity = opData.DataMessage as RoleEntity;
            recordHelper.RecordRole(roleEntity);
        }
    }
}

