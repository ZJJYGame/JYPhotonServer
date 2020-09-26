using AscensionProtocol;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System.Collections.Generic;
using Cosmos;
using System;
namespace AscensionServer
{
    public class AscensionPeer : ClientPeer, IPeer
    {
        #region Properties
        public long SessionId { get; private set; }
        public bool Available { get; private set; }
        public object Handle { get; private set; }
        /// <summary>
        /// 接收消息事件委托
        /// </summary>
        public event Action<byte[]> OnReceiveMessage
        {
            add { onReceiveMessage += value; }
            remove
            {
                try { onReceiveMessage -= value; }
                catch (Exception e) { Utility.Debug.LogError(e); }
            }
        }
        SendParameters sendParam = new SendParameters();
        EventData eventData = new EventData();
        Action<byte[]> onReceiveMessage;
        #endregion
        #region Methods
        public AscensionPeer(InitRequest initRequest) : base(initRequest)
        {
            Handle = this; this.SessionId = ConnectionId;
            Utility.Debug.LogInfo($"Photon SessionId : {SessionId} Available . RemoteAdress:{initRequest.RemoteIP}");
        }
        /// <summary>
        /// 外部接口的发送消息；
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void SendEventMessage(byte opCode, object userData)
        {
            var data = userData as Dictionary<byte, object>;
            eventData.Code = opCode;
            eventData.Parameters = data;
            SendEvent(eventData, sendParam);
        }
        /// <summary>
        /// 发送消息到remotePeer
        /// </summary>
        /// <param name="buffer">缓冲数据</param>
        public void SendMessage(byte[] buffer)
        {
            SendData(buffer, sendParam);
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
            var task = GameManager.CustomeModule<PeerManager>().BroadcastEventAsync((byte)reasonCode, ed);
        }
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            object responseData = GameManager.CustomeModule<NetworkManager>().EncodeMessage(operationRequest);
            var op = responseData as OperationResponse;
            op.OperationCode = operationRequest.OperationCode;
            SendOperationResponse(op, sendParameters);
        }
        protected override void OnReceive(byte[] data, SendParameters sendParameters)
        {
            base.OnReceive(data, sendParameters);
            onReceiveMessage?.Invoke(data);
        }
        #endregion
    }
}