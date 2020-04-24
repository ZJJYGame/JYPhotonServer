using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    public enum ParameterCode : byte//区分传送数据的时候，参数的类型
    {
        Username,
        Password,
        Rolename,
        Gender,//性别
        GoldSpiritRoot,
        WoodSpiritRoot,
        WaterSpiritRoot, 
        FireSpiritRoot,
        SoilSpiritRoot,
        RolePosition,
        x,y,z,
        UsernameList,
        PlayerDataList,
    }
}
