using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using RedisDotNet;

namespace AscensionServer
{
    [CustomeModule]
    public partial class SecondaryJobManager : Module<PetStatusManager>
    {



    }
}
