using System;
using System.Collections.Generic;
using AscensionProtocol;
using Photon.SocketServer;
using Cosmos;
namespace AscensionServer
{
    /// <summary>
    /// 子操作处理者
    /// </summary>
    /// <typeparam name="T">拥有者</typeparam>
    public abstract class SubHandler : ISubHandler
    {
        protected OperationResponse opResponseData = new OperationResponse();

        public Dictionary<byte, object> SubDict { get; protected set; } = new Dictionary<byte, object>();
        //public Handler Owner { get; set; }
        public abstract byte SubOpCode { get; protected set; }
     //   public abstract void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer);
        public abstract OperationResponse EncodeMessage(OperationRequest operationRequest);
        /// <summary>
        /// 获取子操作中的字典对象
        /// 执行此方法时候，会同时执行情况Owner字典与添加子操作码步骤
        /// </summary>
        protected virtual Dictionary<byte, object> ParseSubDict(OperationRequest operationRequest)
        {
            string subDataJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)OperationCode.SubOpCodeData));
            var subDataObj = Utility.Json.ToObject<Dictionary<byte, object>>(subDataJson);
            //Owner.ResponseData.Clear();
            //Owner.OpResponseData.OperationCode = operationRequest.OperationCode;
            //Owner.ResponseData.Add((byte)OperationCode.SubOperationCode, (byte)SubOpCode);
            return subDataObj;
        }
        protected virtual void ResetResponseData(OperationRequest operationRequest)
        {
            //Owner.ResponseData.Clear();
            //Owner.OpResponseData.OperationCode = operationRequest.OperationCode;
            //Owner.ResponseData.Add((byte)OperationCode.SubOperationCode, (byte)SubOpCode);
        }
        protected void SetResponseData(Action callBack)
        {
            //SubDict.Clear();
            callBack?.Invoke();
            //Owner.ResponseData.Add((byte)OperationCode.SubOpCodeData, Utility.Json.ToJson(SubDict));
            //Owner.OpResponseData.Parameters = Owner.ResponseData;
        }
    }
}
