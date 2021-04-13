using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace Protocol
{

    [Serializable]
    [MessagePackObject(true)]
    public sealed class DataParameters: IDataContract
    {
        public object this[byte messageKey]
        {
            get
            {
                object varlue;
                Messages.TryGetValue(messageKey, out varlue);
                return varlue;
            }
            set
            {
                Messages.TryAdd(messageKey, value);
            }
        }
        public byte OperationCode { get; set; }
        public Dictionary<byte, object> Messages { get; set; }
        public short ReturnCode { get; set; }
        public DataParameters() {
            Messages = new Dictionary<byte, object>();
        }
        public DataParameters(byte operationCode)
        {
            this.OperationCode = operationCode;
        }
        public DataParameters(byte operationCode, Dictionary<byte, object> messages) : this(operationCode)
        {
            this.Messages = messages;
        }
        public void SetMessages(Dictionary<byte, object> messages)
        {
            this.Messages = messages;
        }
    }
}
