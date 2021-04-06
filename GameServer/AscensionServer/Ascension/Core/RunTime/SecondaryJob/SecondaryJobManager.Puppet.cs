using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionProtocol;
using AscensionServer.Model;
using RedisDotNet;
using Cosmos;
using Protocol;

namespace AscensionServer
{
    public partial class SecondaryJobManager
    {







        PuppetDTO ChangeDataType(Puppet puppet)
        {
            PuppetDTO puppetDTO = new PuppetDTO();
            puppetDTO.RoleID = puppet.RoleID;
            puppetDTO.Recipe_Array = Utility.Json.ToObject<HashSet<int>>(puppet.Recipe_Array);
            puppetDTO.JobLevel = puppet.JobLevel;
            puppetDTO.JobLevelExp = puppet.JobLevelExp;
            return puppetDTO;
        }
    }
}
