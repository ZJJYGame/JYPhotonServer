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
            GameEntry.RoleManager.OnRoleLogoff+= RecordRole;
        }
         void RecordRole( int roleId)
        {
            recordHelper.RecordRole(roleId);
        }
    }
}

