using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    /// <summary>
    /// 人物属性等相关操作
    /// </summary>
    public enum RoleStatusOPcode
    {
        GetStatus=1,
        UpdateStatus=2,
        Rename=3,
        RestartAddPoint=4,
        SetAddPoint=5,

    }
}
