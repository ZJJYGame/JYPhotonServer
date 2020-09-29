using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    /// <summary>
    /// 数据流对象
    /// </summary>
    public class DataStream:IDisposable
    {
        /// <summary>
        /// 是否正在写入数据；
        /// </summary>
        public bool CanWrite { get; private set; }
        /// <summary>
        /// 是否正在读取数据；
        /// </summary>
        public bool CanRead { get { return !this.CanWrite; } }
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
        int pos;
        public DataStream(bool write, object[] incomingData)
        {
            this.CanWrite = write;
            if (!write && incomingData != null)
                this.readData = incomingData;
        }
        public DataStream(){}
        /// <summary>
        /// 设置读取的流
        /// </summary>
        /// <param name="incomingData">进入流的数据</param>
        /// <param name="pos">序列号</param>
        public void SetReadStream(object[] incomingData, int pos = 0)
        {
            this.readData = incomingData;
            this.pos = pos;
            this.CanWrite = false;
        }
        /// <summary>
        /// 设置写入的流
        /// </summary>
        /// <param name="newWriteData">进入流的数据</param>
        /// <param name="pos">序列号</param>
        internal void SetWriteStream(List<object> newWriteData, int pos = 0)
        {
            if (pos!= newWriteData.Count)
            {
                throw new Exception("SetWriteStream failed, because count does not match position value. sn: " + pos+ " newWriteData.Count:" + newWriteData.Count);
            }
            this.writeData = newWriteData;
            this.pos = pos;
            this.CanWrite = true;
        }
        /// <summary>
        /// 写入状态使用；
        /// 顺序读取写入的下一个顺序；
        /// </summary>
        /// <returns>写入的下一个数据</returns>
        public object ReadNext()
        {
            if (!this.CanRead)
                return null;
            object obj = this.readData[this.pos];
            pos++;
            return obj;
        }
        public void WriteNext(object data)
        {
            if (this.CanWrite)
                writeData.Add(data);
        }
        /// <summary>
        /// 获取接收到的数据数组；
        /// 若写入状态，则获得写入的数据；
        /// 若读取状态，则获得读取的数据；
        /// </summary>
        /// <returns>数据数组</returns>
        public object[] ToArray()
        {
            return this.CanWrite? this.writeData.ToArray() : this.readData;
        }
        public void Dispose()
        {
            readData = null;
            writeData = null;
        }
        public void Clear()
        {
            readData = null;
            writeData .Clear();
            pos = 0;
            CanWrite = true;
        }
    }
}
