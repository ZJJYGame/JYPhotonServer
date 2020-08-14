using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public abstract class ActorBase
    {
        public abstract Type ActorType { get; }
        /// <summary>
        /// 当前Actor是否被激活
        /// </summary>
        public abstract bool IsActive { get; }
        /// <summary>
        /// Actor类型;
        /// 0：玩家；
        /// 1：宠物；
        /// 2：AI类型01
        /// 3：AI类型02
        ///      etc . . . 
        /// 查看 ：ActorTypeEnum
        /// </summary>
        /// </summary>
        public abstract byte ConcreteActorType { get; set; }
        /// <summary>
        /// 系统生成的持久化ID
        /// 可以是Peer的 role id;
        /// 也可以是pet id;
        /// 亦可是80001这类AI怪物
        /// </summary>
        public abstract int ConcreteActorID { get; set; }
    }
}
