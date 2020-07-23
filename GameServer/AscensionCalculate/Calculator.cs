using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
namespace AscensionCalculator
{
    //================================================
    //战斗中的计算器。利用读写分离锁实现线程安全
    // encapsulate  calculator
    //================================================
    public class Calculator:IDisposable
    {

        ReaderWriterLockSlim locker;

        public void Dispose(){}
    }
}
