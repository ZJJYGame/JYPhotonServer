using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 角色数值验证者；
    /// 例：玩家释放技能，服务器验证当前蓝足够时，才允许释放；
    /// 客户端先验证是否可释放，验证通过后由服务器进行再次验证；
    /// </summary>
    public interface IPlayerStatusVerifier
    {
        bool Verified();
    }
}
