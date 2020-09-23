/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : Handler基类
*/
using Photon.SocketServer;
using AscensionProtocol;
using System.Collections.Generic;
using System;
using System.Reflection;
using Cosmos;
using AscensionServer.Threads;
using System.Threading;
namespace AscensionServer
{
    [Inherited]
    public abstract class Handler : IHandler
    {
        #region Properties
        public abstract byte OpCode { get;  }
        protected Dictionary<byte, object> responseParameters;
        protected OperationResponse operationResponse;
        Dictionary<byte, ISubHandler> subHandlerDict;
        #endregion
        #region Methods
        public object EncodeMessage(object message)
        {
            OperationRequest operationRequest = message as OperationRequest;
            return OnOperationRequest(operationRequest);
        }
        public virtual void OnInitialization()
        {
            operationResponse = new OperationResponse();
            responseParameters = new Dictionary<byte, object>();
            subHandlerDict = new Dictionary<byte, ISubHandler>();
        }
        public virtual void OnTermination()
        {
            OnSubHandlerTermination();
        }
        protected virtual OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            var subCode = Convert.ToByte(Utility.GetValue(operationRequest.Parameters, (byte)OperationCode.SubOperationCode));
            if (subCode != 0)
            {
                try
                {
                    ISubHandler subHandler;
                    var result = subHandlerDict.TryGetValue(subCode, out subHandler);
                    if (result)
                         return subHandler.EncodeMessage(operationRequest);
                }
                catch
                {
                    Utility.Debug.LogInfo($"{(OperationCode)operationRequest.OperationCode} ;{ (SubOperationCode)subCode }  has no subHandler ");
                }
            }
            return operationResponse;
        }
        protected virtual void OnSubHandlerInitialization<T>()
            where T : class, ISubHandler
        {
            var subHandlerType = typeof(T);
            Type[] types = Assembly.GetAssembly(subHandlerType).GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (subHandlerType.IsAssignableFrom(types[i]) && types[i].IsClass && !types[i].IsAbstract)
                {
                    var subHandlerResult = Utility.Assembly.GetTypeInstance(types[i]) as T;
                    RegisterSubHandler(subHandlerResult);
                }
            }
        }
        protected virtual void OnSubHandlerTermination()
        {
            foreach (var handler in subHandlerDict)
            {
                DeregisterSubHandler(handler.Value);
            }
        }
        /// <summary>
        /// 非空虚函数
        /// </summary>
        /// <param name="operationRequest"></param>
        protected void SetRequestData(OperationRequest operationRequest)
        {
            responseParameters.Clear();
            operationResponse.OperationCode = operationRequest.OperationCode;
        }
        void RegisterSubHandler(ISubHandler handler)
        {
           var result= subHandlerDict.TryAdd(handler.SubOpCode, handler);
            if(!result)
                Utility.Debug.LogError("重复键值：\n" + handler.ToString() + "\n:" + handler.SubOpCode.ToString() + "\n结束");
        }
        void DeregisterSubHandler(ISubHandler handler)
        {
            subHandlerDict.Remove((byte)handler.SubOpCode);
        }
        #endregion
    }
}
