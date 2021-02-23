using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;

namespace AscensionServer
{
    public class SyncWeaponHandler:Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncWeapon; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncWeaponSubHandler>();
        }
    }
}


