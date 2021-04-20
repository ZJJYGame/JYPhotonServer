using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
namespace AscensionServer
{
    [Serializable]
    [MessagePackObject(true)]
    public class OperationData : IDisposable
    {
        /// <summary>
        /// 主操作码；
        /// </summary>
        public ushort OperationCode { get; set; }
        /// <summary>
        /// 这里用来存放数据；
        /// </summary>
        public object DataMessage { get; set; }
        public short ReturnCode { get; set; }
        /// <summary>
        /// 子操作码
        /// </summary>
        public short SubOperationCode { get; set; }
        public OperationData() { }
        public OperationData(ushort operationCode)
        {
            OperationCode = operationCode;
        }
        public virtual OperationData Clone()
        {
            return new OperationData()
            {
                OperationCode = this.OperationCode,
                ReturnCode = this.ReturnCode,
                DataMessage = this.DataMessage,
                SubOperationCode = this.SubOperationCode
            };
        }
        public virtual void Dispose()
        {
            OperationCode = 0;
            ReturnCode = 0;
            DataMessage = null;
            SubOperationCode = 0;
        }
    }
}
