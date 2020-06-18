using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer.Threads
{
    public class ThreadData<T>:IThreadData
    {
        public ICollection<T> Collection { get; private set; }
        public byte EventCode { get; private set; }
        public T Data { get; private set; }
        public ThreadData()
        {
            Collection = null;
            EventCode = 0;
            Data = default(T);
        }
        public ThreadData(ICollection<T> collection, byte eventCode, T data)
        {
            this.Collection = collection;
            this.EventCode = eventCode;
            this.Data = data;
        }
        public void SetData(ICollection<T> collection, byte eventCode, T data)
        {
            this.Collection = collection;
            this.EventCode = eventCode;
            this.Data = data;
        }
        public void Clear()
        {
            Collection = null;
            EventCode = 0;
            Data = default(T);
        }
    }
}
