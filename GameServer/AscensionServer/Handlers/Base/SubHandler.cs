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
    public abstract class SubHandler : ISubHandler
    {
        public abstract byte SubOpCode { get; protected set; }
        /// <summary>
        /// 操作返回数据
        /// </summary>
        protected OperationResponse operationResponse = new OperationResponse();
        /// <summary>
        /// 子操作码的返回数据字典
        /// </summary>
        protected Dictionary<byte, object> subResponseParameters= new Dictionary<byte, object>();
        /// <summary>
        /// 编码消息；
        /// </summary>
        /// <param name="operationRequest">请求进来的数据</param>
        /// <returns></returns>
        public abstract OperationResponse EncodeMessage(OperationRequest operationRequest);
        /// <summary>
        /// 获取子操作中的字典对象
        /// </summary>
        protected virtual Dictionary<byte, object> ParseSubParameters(OperationRequest operationRequest)
        {
            string subDataJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)OperationCode.SubOpCodeData));
            var subDataObj = Utility.Json.ToObject<Dictionary<byte, object>>(subDataJson);
            subResponseParameters.Clear();
            operationResponse.OperationCode = operationRequest.OperationCode;
            subResponseParameters.Add((byte)OperationCode.SubOperationCode, SubOpCode);
            return subDataObj;
        }
        /// <summary>
        /// 重置返回数据
        /// </summary>
        /// <param name="operationRequest">发送来的数据</param>
        protected virtual void ResetResponseData(OperationRequest operationRequest)
        {
            subResponseParameters.Clear();
            operationResponse.OperationCode = operationRequest.OperationCode;
            subResponseParameters.Add((byte)OperationCode.SubOperationCode, SubOpCode);
        }
        /// <summary>
        /// 设置返回数据
        /// </summary>
        /// <param name="callBack">在回调中设置数据</param>
        protected void SetResponseParamters(Action callBack)
        {
            subResponseParameters.Clear();
            callBack?.Invoke();
            subResponseParameters.Add((byte)OperationCode.SubOpCodeData, Utility.Json.ToJson(subResponseParameters));
            operationResponse.Parameters = subResponseParameters;
        }
    }
}
