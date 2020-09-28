using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace AscensionServer
{
    public class DataStreamQueue
    {
        /// <summary>
        /// 发送间隔
        /// </summary>
        int snd_rate;
        /// <summary>
        /// 消息总数
        /// </summary>
        int packetCount;
        /// <summary>
        /// 每个数据包所包含的消息数；
        /// </summary>
        int objectPerPacket;
        /// <summary>
        /// 时间戳
        /// </summary>
        long ts;
        /// <summary>
        /// 下一个接收的序号
        /// </summary>
        int rcv_nxt;
        /// <summary>
        /// 是否可写入；
        /// </summary>
        bool isWriting;
        List<object> objectSet = new List<object>();
        public DataStreamQueue(int sndRate)
        {
            this.snd_rate = sndRate;
        }
        public void Reset()
        {
            snd_rate = 0;
            objectPerPacket = -1;
            ts = 0;
            objectSet.Clear();
        }
        public void WriteNext(object obj)
        {
            if (Utility.Time.MillisecondTimeStamp() != ts)
                WritePackage();
            ts = Utility.Time.MillisecondTimeStamp();
            if (isWriting)
                objectSet.Add(obj);
        }
        public object ReadNext()
        {
            if (rcv_nxt == -1)
                return null;
            if (rcv_nxt >= objectSet.Count)
                rcv_nxt -= objectPerPacket;
            return objectSet[rcv_nxt++];
        }
        public void Serialize(DataStream stream)
        {
            if (objectSet.Count > 0 && objectPerPacket < 0)
                objectPerPacket = objectSet.Count;
            stream.WriteNext(packetCount);
            stream.WriteNext(objectPerPacket);
            for (int i = 0; i < objectSet.Count; i++)
                stream.WriteNext(objectSet[i]);
            objectSet.Clear();
            packetCount = 0;
        }
        public void Deserialize(DataStream stream)
        {
            objectSet.Clear();
            packetCount = (int)stream.ReadNext();
            objectPerPacket =(int) stream.ReadNext();
            var length = packetCount * objectPerPacket;
            for (int i = 0; i < length; ++i)
                objectSet.Add(stream.ReadNext());
            if (objectSet.Count > 0)
                rcv_nxt = 0;
            else
                rcv_nxt = -1;
        }
        void WritePackage()
        {
            if (Utility.Time.MillisecondTimeStamp() < ts + 1000 / snd_rate)
            {
                isWriting = false;
                return;
            }
            if (packetCount == 1)
                objectPerPacket = objectSet.Count;
            else if (packetCount>1)
            {
                if (objectSet.Count / packetCount != objectPerPacket)
                {
                    Utility.Debug.LogError("The number of objects sent via a DataStreamQueue has to be the same each frame");
                }
            }
            isWriting = true;
            packetCount++;
            ts = Utility.Time.MillisecondTimeStamp();
        }
    }
}
