using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionProtocol
{
    /// <summary>
    /// 数据流对象
    /// </summary>
    public class DataStream:IRenewable
    {
        /// <summary>
        /// 是否正在写入数据；
        /// </summary>
        public bool IsWriting { get; private set; }
        /// <summary>
        /// 是否正在读取数据；
        /// </summary>
        public bool IsReading { get { return !this.IsWriting; } }
        /// <summary>
        /// 写入的数据；
        /// </summary>
        List<object> writeData;
        public List<object> WriteData { get { return writeData; } }
        /// <summary>
        /// 读取的数据；
        /// </summary>
        object[] readData;
        /// <summary>
        /// serial number
        ///当前 data序号
        /// </summary>
        int sn; 
        public DataStream(bool write, object[] incomingData)
        {
            this.IsWriting = write;
            if (!write && incomingData != null)
                this.readData = incomingData;
        }
        /// <summary>
        /// 设置读取的流
        /// </summary>
        /// <param name="incomingData">进入流的数据</param>
        /// <param name="sn">序列号</param>
        public void SetReadStream(object[] incomingData, int sn = 0)
        {
            this.readData = incomingData;
            this.sn = sn;
            this.IsWriting = false;
        }
        /// <summary>
        /// 设置写入的流
        /// </summary>
        /// <param name="newWriteData">进入流的数据</param>
        /// <param name="sn">序列号</param>
        internal void SetWriteStream(List<object> newWriteData, int sn = 0)
        {
            if (sn!= newWriteData.Count)
            {
                throw new Exception("SetWriteStream failed, because count does not match position value. sn: " + sn+ " newWriteData.Count:" + newWriteData.Count);
            }
            this.writeData = newWriteData;
            this.sn = sn;
            this.IsWriting = true;
        }
        /// <summary>
        /// 只读状态使用；
        /// 顺序读取接收的下一个数据；
        /// </summary>
        /// <returns></returns>
        public object ReadReceivedNext()
        {
            if (this.IsWriting)
                return null;
            object obj = this.readData[this.sn];
            this.sn++;
            return obj;
        }
        /// <summary>
        /// 写入状态使用；
        /// 顺序读取写入的下一个顺序；
        /// </summary>
        /// <returns>写入的下一个数据</returns>
        public object ReadWritedNext()
        {
            if (this.IsWriting)
                return null;
            object obj = this.readData[this.sn];
            return obj;
        }
        /// <summary>
        /// 获取接收到的数据数组；
        /// 若写入状态，则获得写入的数据；
        /// 若读取状态，则获得读取的数据；
        /// </summary>
        /// <returns>数据数组</returns>
        public object[] ToArray()
        {
            return this.IsWriting ? this.writeData.ToArray() : this.readData;
        }
        public void OnRenewal()
        {
        }
        public byte[] Serialize()
        {
            if (this.IsWriting)
            { }
            return null;
        }
        public void Write(object data)
        {
            if (this.IsWriting)
            {
                writeData.Add(data);
            }
            else
            {
                if (readData.Length > sn)
                {

                }
            }
        }
        public object[] Deserialize(byte[] buffer)
        {
            if (this.IsWriting)
            { }
            return null;
        }
    }
}
