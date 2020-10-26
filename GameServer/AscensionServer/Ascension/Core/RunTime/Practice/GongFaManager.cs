using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionData;
using AscensionProtocol.DTO;
using UnityEngine;

namespace AscensionServer
{
    [CustomeModule]
   public class GongFaManager : Module<GongFaManager>
    {
        public Dictionary<int, GongFa> gongfaDataDict = new Dictionary<int, GongFa>();

        public GongFaManager()
        {


        }




    }
}
