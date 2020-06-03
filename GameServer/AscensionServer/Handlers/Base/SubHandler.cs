using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;

namespace AscensionServer
{
    /// <summary>
    /// 子操作处理者
    /// </summary>
    /// <typeparam name="T">拥有者</typeparam>
    public abstract class SubHandler: ISubHandler
    {
        public Handler Owner { get; set; }
        public SubOperationCode SubOpCode { get; protected set; }
        public abstract void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer);
        public virtual void OnInitialization() {SubDict = new Dictionary<byte, object>();}
        public virtual void OnTermination() { }
        /// <summary>
        /// 获取子操作中的字典对象
        /// 执行此方法时候，会同时执行情况Owner字典与添加子操作码步骤
        /// </summary>
        protected virtual Dictionary<byte,object> InitSubOpDict(OperationRequest operationRequest)
        {
            string subDataJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)OperationCode.SubOpCodeData));
            var subDataObj = Utility.ToObject<Dictionary<byte, object>>(subDataJson);
            Owner.ResponseData.Clear();
            Owner.OpResponse.OperationCode = operationRequest.OperationCode;
            Owner.ResponseData.Add((byte)OperationCode.SubOperationCode, (byte)SubOpCode);
            SubDict.Clear();
            return subDataObj;
        }
        public Dictionary<byte, object> SubDict { get; protected set; }
    }
}
