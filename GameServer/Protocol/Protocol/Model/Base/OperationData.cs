﻿using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
namespace Protocol
{
    [Serializable]
    [MessagePackObject(true)]
    public class OperationData : IDisposable
    {
        public ushort OperationCode { get; set; }
        /// <summary>
        /// 这里用来存放MessagePack数据；
        /// </summary>
        public IDataContract DataContract { get; set; }
        /// <summary>
        /// 这里用来存放Json数据；
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
                DataContract = this.DataContract,
                OperationCode = this.OperationCode,
                ReturnCode = this.ReturnCode,
                DataMessage = this.DataMessage,
                SubOperationCode = this.SubOperationCode
            };
        }
        public virtual void Dispose()
        {
            OperationCode = 0;
            DataContract = null;
            ReturnCode = 0;
            DataMessage = null;
            SubOperationCode = 0;
        }
    }
}
