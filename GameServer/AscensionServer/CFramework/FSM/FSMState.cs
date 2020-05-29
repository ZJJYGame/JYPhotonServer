/*
*Author : ZJD
*Since 	:2020-05-29
*Description  : 简易的有限状态机状态基类
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public abstract class FSMState<T> : IReference,IBehaviour
         where T : class
    {
        public abstract void Clear();
        public abstract void OnInitialization();
        public abstract void OnTermination();
    }
}
