using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer.Threads
{
    /// <summary>
    /// 泛型数据传输容器
    /// 开放继承
    /// </summary>
    public  class LogicEventArgs<T> : EventArgs, IEventArgs
    {
        /// <summary>
        /// 泛型构造
        /// </summary>
        /// <param name="data"></param>
        public LogicEventArgs(T data)
        {
            SetData(data);
        }
        public LogicEventArgs() { }
        /// <summary>
        /// 泛型数据类型
        /// </summary>
        public T Data { get; private set; }
        public LogicEventArgs<T> SetData(T data)
        {
            Data = data;
            return this;
        }
        public  virtual void Clear()
        {
            Data = default(T);
        }
    }
}