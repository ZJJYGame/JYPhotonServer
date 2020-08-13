using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 并发变量容器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrentVariable<T> 
        where T : new()
    {
        static readonly object locker = new object();
        T data = new T();
        public T Data
        {
            get
            {
                lock (locker)
                {
                    return data;
                }
            }
            set
            {
                lock (locker)
                {
                    data = value;
                }
            }
        }
    }
}
