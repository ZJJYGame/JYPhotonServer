using AscensionProtocol;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System.Collections.Generic;
using Cosmos;
using System;
using System.Text;

namespace AscensionServer
{
    public class AscensionPeer : ClientPeer, IAscensionPeer
    {
        #region Properties
        public int SessionId { get; private set; }
        public bool Available { get; private set; }
        public object Handle { get; private set; }
        /// <summary>
        /// 接收消息事件委托；
        /// 此类消息为非Opcode类型；
        /// </summary>
        public event Action<object> OnMessageReceive
        {
            add { onMessageReceive += value; }
            remove
            {
                try { onMessageReceive -= value; }
                catch (Exception e) { Utility.Debug.LogError(e); }
            }
        }
        SendParameters sendParam = new SendParameters();
        EventData eventData = new EventData();
        Action<object> onMessageReceive;
        #endregion
        #region Methods
        public AscensionPeer(InitRequest initRequest) : base(initRequest)
        {
            Handle = this; this.SessionId = ConnectionId;
            Utility.Debug.LogInfo($"Photon SessionId : {SessionId} Available . RemoteAdress:{initRequest.RemoteIP}");
        }
        /// <summary>
        /// 发送消息到remotePeer
        /// </summary>
        public void SendMessage(object message)
        {
            base.SendMessage(message, sendParam);
        }
        /// <summary>
        /// 发送事件消息;
        /// 传输的数据类型限定为Dictionary<byte,object>类型；
        /// </summary>
        /// <param name="data">用户自定义数据</param>
       public void SendEventMsg(byte opCode, object data)
        {
            eventData.Code = opCode;
            eventData.Parameters = data as Dictionary<byte, object>;
            base.SendEvent(eventData, sendParam);
        }
        public void Clear()
        {
            SessionId = 0;
            Available = false;
            Handle = null;
        }
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            EventData ed = new EventData((byte)EventCode.DeletePlayer);
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            ed.Parameters = data;
            GameManager.CustomeModule<PeerManager>().TryRemove(SessionId);
            Utility.Debug.LogError($"Photon SessionId : {SessionId} Unavailable . RemoteAdress:{RemoteIPAddress}");
            var task = GameManager.CustomeModule<PeerManager>().BroadcastEventToAllAsync((byte)reasonCode, ed);
        }
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            operationRequest.Parameters.Add((byte)ParameterCode.ClientPeer,this);
            object responseData = GameManager.CustomeModule<NetworkManager>().EncodeMessage(operationRequest);
            var op = responseData as OperationResponse;
            op.OperationCode = operationRequest.OperationCode;
            SendOperationResponse(op, sendParameters);
        }
        /// <summary>
        /// 接收到客户端消息；
        /// </summary>
        protected override void OnMessage(object message, SendParameters sendParameters)
        {
            //接收到客户端消息后，进行委托广播；
            onMessageReceive?.Invoke(message);
        }
        #endregion
    }
}