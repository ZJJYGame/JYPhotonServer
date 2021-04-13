using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace AscensionServer
{
    /// <summary>
    /// 二进制数据包，用于整合键值对数据；
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public struct BinaryParameters
    {
        public byte[] this[byte messageKey]
        {
            get
            {
                byte[] varlue;
                Messages.TryGetValue(messageKey, out varlue);
                return varlue;
            }
            set
            {
                Messages.TryAdd(messageKey, value);
            }
        }
        public Dictionary<byte, byte[]> Messages { get; set; }
        public static BinaryParameters None { get { return new BinaryParameters();  } }
    }
}
