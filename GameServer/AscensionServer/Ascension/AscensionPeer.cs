/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 客户端类
*/
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
        public event Action<byte[]> ReceiveMessage
        {
            add { receiveMessage += value; }
            remove
            {
                try{receiveMessage -= value;}
                catch (Exception e){Utility.Debug.LogError(e);}
            }
        }
        SendParameters sendParam = new SendParameters();
        EventData eventData = new EventData();
        Action<byte[]> receiveMessage;
        #endregion

        #region Methods
        public AscensionPeer(InitRequest initRequest) : base(initRequest) { Handle = this; }

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
            var t = GameManager.CustomeModule<PeerManager>().BroadcastEventAsync((byte)OperationCode.Logoff, ed);
        }
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            object responseData = GameManager.CustomeModule<NetworkManager>().EncodeMessage(operationRequest);
            SendOperationResponse(responseData as OperationResponse, sendParameters);
        }
        protected override void OnReceive(byte[] data, SendParameters sendParameters)
        {
            base.OnReceive(data, sendParameters);
            receiveMessage?.Invoke(data);
        }
        #endregion
    }
}