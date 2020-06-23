using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 状态模板类
    /// 
    /// 1.绑定状态拥有者
    /// 2.便于访问状态拥有者特性（便于执行动画播放）
    /// </summary>
   public  class FSMStateTemplate<OwnerType>:IFSMStateBase
    {
        protected OwnerType mStateOwner;

        public FSMStateTemplate(int _id,OwnerType _ownerType) : base(_id)
        {
            mStateOwner = _ownerType;
        }
    }
}
