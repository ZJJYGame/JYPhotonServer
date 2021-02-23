using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    public interface IGongFaManager:IModuleManager
    {
        List<int> GetSkillList(int level, int gongfaid);
        GongFa GetGngFa(int gongfaid);
    }
}


