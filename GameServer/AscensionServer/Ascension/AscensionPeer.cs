/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 客户端类
*/
using AscensionProtocol;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System.Collections.Generic;
using AscensionServer.Model;
using AscensionProtocol.DTO;
namespace AscensionServer
{
    //管理跟客户端的链接的
    public class AscensionPeer : ClientPeer
    {
        /// <summary>
        /// 一个Peer保存当前登录的用户
        /// </summary>
        User user;
        public User User { get { if (user == null) user = new User(); return user; } set { user = value; } }
        OnlineStatusDTO onlineStateDTO;
        public OnlineStatusDTO OnlineStateDTO { get { return onlineStateDTO; } set { onlineStateDTO = value; } }
        public float x, y, z;
        /// <summary>
        /// Peer的UUID
        /// </summary>
        public string PeerGUID { get; set; }
        public AscensionPeer(InitRequest initRequest) : base(initRequest) { }
        //处理客户端断开连接的后续工作
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            #region Lagcy
            //////告诉其他客户端有客户端断开链接    
            //foreach (AscensionPeer temPeer in AscensionServer.Instance.PeerList)
            //{
            //    if (!string.IsNullOrEmpty(temPeer.Account)&&temPeer.Account!=Account)
            //    {
            //        EventData ed = new EventData((byte)EventCode.DeletePlayer);
            //        Dictionary<byte, object> data = new Dictionary<byte, object>();
            //        data.Add((byte)ParameterCode.UserCode.Username,Account);
            //        ed.Parameters = data;
            //        temPeer.SendEvent(ed, new SendParameters()); //发送事件sendParameters                
            //    }
            //}
            //AscensionServer.Instance.PeerList.Remove(this);
            #endregion
            EventData ed = new EventData((byte)EventCode.DeletePlayer);
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ObjectParameterCode.User,onlineStateDTO);
            ed.Parameters = data;
            AscensionServer.Instance.Logoff(this);
            foreach (AscensionPeer tmpPeer in AscensionServer.Instance.ConnectedPeerHashSet)
            {
                tmpPeer.SendEvent(ed, new SendParameters());             
            }
            AscensionServer.log.Info("~~~~~~~~~~~~~~~~~~~~~~~~~~~ PeerIP  " + RemoteIP + "  Disconnected~~~~~~~~~~~~~~~~~~~~~~~~");
        }
        //处理客户端的请求
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            //SendParameters 参数，传递的数据
            AscensionServer.log.Info("~~~~~~~~~~~~~~~~~~~~~~~~~~~ PeerIP  " + RemoteIP + "  Request  ~~~~~~~~~~~~~~~~~~~~~~~~");
            //通过客户端的OperationCode从HandlerDict里面获取到了需要的Hander
            BaseHandler handler = Utility.GetValue(AscensionServer.Instance.HandlerDict, (OperationCode)operationRequest.OperationCode);
            //如果找到了需要的hander就调用我们hander里面处理请求的方法
            if (handler != null)
            {
                handler.OnOperationRequest(operationRequest, sendParameters, this);
            }
            else//否则我们就使用默认的hander
            {
                BaseHandler defaultHandler = Utility.GetValue<OperationCode, BaseHandler>(AscensionServer.Instance.HandlerDict, OperationCode.Default);
                defaultHandler.OnOperationRequest(operationRequest, sendParameters, this);
            }
        }
        public void Login(User user)
        {
            this.User.Account = user.Account;
            this.User.UUID = user.UUID;
            this.User.Password = user.Password;
        }
        public void Logoff()
        {
            this.User.Account = null;
            this.User.UUID = null;
            this.User.Password = null;
        }
        public override string ToString()
        {
            return "######Account : " + User.Account + " ; Password : " + User.Password + "; UUID : " + User.UUID+"######";
        }
    }
 }
