using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    public interface IDemonicSoulManager:IModuleManager
    {
        void AddDemonical(int roleid, DemonicSoul demonicSoul, int soulid, NHCriteria nHCriteria);
        void CompoundDemonical(List<int> soulList, DemonicSoul demonicSoul, int roleid, NHCriteria nHCriteria);

        void GetDemonicSoul(int roleid, DemonicSoul demonicSoul);

        void S2CDemonicalMessage(int roleid, string message, ReturnCode returnCode);
    }
}


