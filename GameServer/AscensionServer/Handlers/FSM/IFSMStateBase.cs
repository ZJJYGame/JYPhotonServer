using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// FSM 状态接口基类
/// </summary>
namespace AscensionServer
{
    public abstract class IFSMStateBase
    {
        public int mStateId = default(int);

        public FSMStateMachineMgr fSMStateMachineMgr;

        public IFSMStateBase(int _id)
        {
            mStateId = _id;
        }

        /// <summary>
        /// 状态进入
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnStateEnter(params object[] args) { }

        /// <summary>
        /// 状态停留
        /// </summary>
        public virtual void OnStateStay(params object[] args) { }

        /// <summary>
        /// 状态退出
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnStateExit(params object[] args) { }
    }
}
