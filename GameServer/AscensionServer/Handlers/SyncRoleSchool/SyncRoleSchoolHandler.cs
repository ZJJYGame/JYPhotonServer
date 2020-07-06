using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
namespace AscensionServer
{
   public class SyncRoleSchoolHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncRoleSchool;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncRoleSchoolSubHandler>();
        }
    }
}
