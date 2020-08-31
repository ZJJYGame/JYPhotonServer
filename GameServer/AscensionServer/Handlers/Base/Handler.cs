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
    public abstract class  Handler:IHandler
    {
        #region Properties
        public OperationCode OpCode { get; protected set; }
        public EventCode EvCode { get; protected set; }
         Dictionary<byte, ISubHandler> subHandlerDict;
        public OperationResponse OpResponse { get; protected set; }
        public Dictionary<byte, object> ResponseData { get; protected set; }
        protected Dictionary<byte, object> threadEventParameter { get; set; }
        #endregion

        #region Methods
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
                    Utility.Debug.LogInfo($"{(OperationCode)operationRequest.OperationCode} ;{ (SubOperationCode)subCode }  has no subHandler ");
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
        public void ResetHandler()
        {
            ResponseData.Clear();
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
                   Utility.Debug.LogInfo($" {subHandlerResult.GetType().FullName } :  OnSubHandlerInitialization ");
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
        /// 执行线程事件；
        /// 线程执行结束后，会自动回收线程数据对象，并清空线程事件参数字典
        /// </summary>
        /// <param name="peerCollection"></param>
        /// <param name="eventCode"></param>
        /// <param name="parameters"></param>
        protected void QueueThreadEvent(ICollection<AscensionPeer> peerCollection,EventCode eventCode,Dictionary<byte,object>parameters,string message=null)
        {
            //利用池生成线程池所需要使用的对象，并为其赋值，结束时回收
            var threadEventData =  new ThreadEventData();
            threadEventData.SetValue(peerCollection, (byte)eventCode);
            threadEventData.SetData(parameters);
            var threadSyncEvent =new ThreadSyncEvent();
            threadSyncEvent.OnInitialization();
            threadSyncEvent.SetEventData(threadEventData);
            threadSyncEvent.AddFinishedHandler(() => {
                AscensionServer._Log.Info(message);
                threadSyncEvent.Clear();
            });
            ThreadPool.QueueUserWorkItem(threadSyncEvent.Handler);
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


        #endregion
    }
}
