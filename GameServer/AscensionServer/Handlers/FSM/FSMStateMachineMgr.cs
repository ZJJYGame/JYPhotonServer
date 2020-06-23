using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 状态拥有者:状态管理类
/// </summary>
namespace AscensionServer
{
    public class FSMStateMachineMgr
    {
        private IFSMStateBase mCur_State;
        private IFSMStateBase mHis_State;
        /// <summary>
        /// 状态缓存池
        /// </summary>
        private Dictionary<int, IFSMStateBase> mState_Dic = new Dictionary<int, IFSMStateBase>();

        public FSMStateMachineMgr(IFSMStateBase _state)
        {
            mHis_State = null;
            mCur_State = _state;
            RegisterState(_state);
            mCur_State.OnStateEnter();
        }
        /// <summary>
        /// 注册状态
        /// </summary>
        /// <param name="iFSMState"></param>
        public void RegisterState(IFSMStateBase iFSMState)
        {
            if (mState_Dic.ContainsKey(iFSMState.mStateId))
                return;
            mState_Dic.Add(iFSMState.mStateId, iFSMState);
            iFSMState.fSMStateMachineMgr = this;
        }
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="_stateId"></param>
        public void TranslateToState(int _stateId)
        {
            if (!mState_Dic.ContainsKey(_stateId))
                return;

            mCur_State.OnStateExit();
            mHis_State = mCur_State;
            mCur_State = mState_Dic[_stateId];
            mCur_State.OnStateEnter();
        }
        /// <summary>
        /// 状态更新，固定时间间隔调用
        /// </summary>
        public void StateUpdate()
        {
            if (mCur_State !=null)
                mCur_State.OnStateStay();
        }
    }
}
