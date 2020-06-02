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
namespace AscensionServer
{
    public abstract class  Handler:IHandler
    {
        public OperationCode OpCode { get; protected set; }
        public EventCode EvCode { get; protected set; }
         Dictionary<SubOperationCode, ISubHandler> subHandlerDict;
        public OperationResponse OpResponse { get; protected set; }
        public Dictionary<byte, object> ResponseData { get; protected set; }
        public virtual  void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters,AscensionPeer peer)
        {
            var subCode = (SubOperationCode)Utility.GetValue(operationRequest.Parameters, (byte)OperationCode.SubOperationCode);
            if (subCode != SubOperationCode.None)
            {
                try
                {
                    ISubHandler subHandler;
                    subHandlerDict.TryGetValue(subCode, out subHandler);
                    subHandler.Handler(operationRequest, sendParameters, peer);
                }
                catch
                {
                    AscensionServer._Log.Info(">>>>>\n" + subCode.ToString() + "has no subHandler \n<<<<<");
                }
            }
                  
        }
        public virtual void OnInitialization()
        {
            OpResponse = new OperationResponse();
            ResponseData = new Dictionary<byte, object>();
            subHandlerDict = new Dictionary<SubOperationCode, ISubHandler>();
            AscensionServer.Instance.RegisterHandler(this);
        }
        public virtual void OnTermination()
        {
            AscensionServer.Instance.DeregisterHandler(this);
            OnSubHandlerTermination();
        }
        protected virtual void OnSubHandlerInitialization<T>()
            where T:class, ISubHandler
        {
            var subHandlerType = typeof(T);
            Type[] types = Assembly.GetAssembly(subHandlerType).GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (subHandlerType.IsAssignableFrom(types[i]) && types[i].IsClass && !types[i].IsAbstract)
                {
                    var subHandlerResult = Utility.Assembly.GetTypeInstance(types[i]) as T;
                    subHandlerResult.Owner = this;
                    subHandlerResult.OnInitialization();
                    RegisterSubHandler(subHandlerResult);
                }
            }
        }
        protected virtual void OnSubHandlerTermination()
        {
            foreach (var handler in subHandlerDict)
            {
                handler.Value.OnTermination();
                DeregisterSubHandler(handler.Value);
            }
        }
        void RegisterSubHandler(ISubHandler handler)
        {
            if (subHandlerDict.ContainsKey(handler.SubOpCode))
                AscensionServer._Log.Info("重复键值：\n" + handler.ToString() + "\n:" + handler.SubOpCode.ToString() + "\n结束");
            subHandlerDict.Add(handler.SubOpCode, handler);
        }
        void DeregisterSubHandler(ISubHandler handler)
        {
            subHandlerDict.Remove(handler.SubOpCode);
        }
    }
}
