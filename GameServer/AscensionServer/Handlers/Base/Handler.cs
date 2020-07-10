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

namespace AscensionServer
{
    public abstract class  Handler:IHandler
    {
        public OperationCode OpCode { get; protected set; }
        public EventCode EvCode { get; protected set; }
         Dictionary<byte, ISubHandler> subHandlerDict;
        public OperationResponse OpResponse { get; protected set; }
        public Dictionary<byte, object> ResponseData { get; protected set; }
        protected Dictionary<byte, object> threadEventParameter { get; set; }
        public virtual  void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters,AscensionPeer peer)
        {
            var subCode = Convert.ToByte( Utility.GetValue(operationRequest.Parameters, (byte)OperationCode.SubOperationCode));
            if (subCode != 0)
            {
                try
                {
                    ISubHandler subHandler;
                    var result=  subHandlerDict.TryGetValue(subCode, out subHandler);
                    if (result)
                        subHandler.Handler(operationRequest, sendParameters, peer);
                }
                catch
                {
                    AscensionServer._Log.Info(">>>>>\n" +(OperationCode)operationRequest.OperationCode+" ; " + (SubOperationCode)subCode + " :  has no subHandler \n<<<<<");
                }
            }
        }
        public virtual void OnInitialization()
        {
            OpResponse = new OperationResponse();
            ResponseData = new Dictionary<byte, object>();
            subHandlerDict = new Dictionary<byte, ISubHandler>();
            AscensionServer.Instance.RegisterHandler(this);
            threadEventParameter = new Dictionary<byte, object>();
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
                    AscensionServer._Log.Info(">>>>> \n " + subHandlerResult.GetType().FullName + " :  OnSubHandlerInitialization \n<<<<<");
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
            if (subHandlerDict.ContainsKey((byte)handler.SubOpCode))
                AscensionServer._Log.Info("重复键值：\n" + handler.ToString() + "\n:" + handler.SubOpCode.ToString() + "\n结束");
            subHandlerDict.Add((byte)handler.SubOpCode, handler);
        }
        void DeregisterSubHandler(ISubHandler handler)
        {
            subHandlerDict.Remove((byte)handler.SubOpCode);
        }
        /// <summary>
        /// 非空虚函数
        /// </summary>
        /// <param name="operationRequest"></param>
        protected void SetRequestData(OperationRequest operationRequest)
        {
            ResponseData.Clear();
            OpResponse.OperationCode = operationRequest.OperationCode;
        }
        /// <summary>
        /// 派发线程事件
        /// </summary>
        /// <param name="peerCollection"></param>
        /// <param name="eventCode"></param>
        /// <param name="parameters"></param>
        protected void ExecuteThreadEvent(ICollection<AscensionPeer> peerCollection,EventCode eventCode,Dictionary<byte,object>parameters)
        {
            //利用池生成线程池所需要使用的对象，并为其赋值，结束时回收
            var threadEventData = Singleton<ReferencePoolManager>.Instance.Spawn<ThreadEventData>();
            threadEventData.SetValue(peerCollection, (byte)eventCode);
            threadEventData.SetData(parameters);
            var threadSyncEvent = Singleton<ReferencePoolManager>.Instance.Spawn<ThreadSyncEvent>();
            threadSyncEvent.SetEventData(threadEventData);
            threadSyncEvent.AddFinishedHandler(() => {
                Singleton<ReferencePoolManager>.Instance.Despawns(threadSyncEvent, threadEventData);
                ThreadEvent.RemoveSyncEvent(threadSyncEvent);
            });
            ThreadEvent.AddSyncEvent(threadSyncEvent);
            ThreadEvent.ExecuteEvent();
        }
    }
}
