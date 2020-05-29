/*
*Author : ZJD
*Since 	:2020-05-29
*Description  : 简易的有限状态机，用于处理服务器接收到的请求事务
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.FSM
{
    public sealed class FSM<T>:IReference
        where T:class
    {
         T onwer;
        public T Onwer { get { return onwer; }private set { onwer = value; } }
        public void Clear(){onwer = null;handlerDict.Clear(); }
        Dictionary<int, IHandler> handlerDict = new Dictionary<int, IHandler>();
        IHandler currentHandler;
        public void EnterState()
        {
        }
        public void ExitState()
        {
        }
        public void ChangeState(int triggerID)
        {
        }
        public void AddTriggerStates(int trigger,IHandler state)
        {

        }
    }
}
