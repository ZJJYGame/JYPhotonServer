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
using System;
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
            
            EventData ed = new EventData((byte)EventCode.DeletePlayer);
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ObjectParameterCode.User,onlineStateDTO);
            ed.Parameters = data;
            if (this.User.Account != null)
            {
                if (AscensionServer.Instance.HasOnlineID(this) != 0)
                {
                    RecordOnOffLine(AscensionServer.Instance.HasOnlineID(this));
                }
                AscensionServer.Instance.offline(this);
            }

            AscensionServer.Instance.Logoff(this);
            foreach (AscensionPeer tmpPeer in AscensionServer.Instance.ConnectedPeerHashSet)
            {
                tmpPeer.SendEvent(ed, new SendParameters());                      
            }
           

        }
        //处理客户端的请求
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            //SendParameters 参数，传递的数据
            //通过客户端的OperationCode从HandlerDict里面获取到了需要的Hander
            Handler handler = Utility.GetValue(AscensionServer.Instance.HandlerDict, (OperationCode)operationRequest.OperationCode);
            //如果找到了需要的hander就调用我们hander里面处理请求的方法
            if (handler != null)
            {
                handler.OnOperationRequest(operationRequest, sendParameters, this);
            }
            else//否则我们就使用默认的hander
            {
                Handler defaultHandler = Utility.GetValue<OperationCode, Handler>(AscensionServer.Instance.HandlerDict, OperationCode.Default);
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
        //记录客户端离线时间
        protected void RecordOnOffLine(int roleid)
        {
            AscensionServer._Log.Info("同步离线时间成功");
            OnOffLineDTO onOffLine = new OnOffLineDTO() { RoleID = roleid };           
            string onofflineJson = Utility.ToJson(onOffLine);
            //////
            var onofflinetemp = Utility.ToObject<OnOffLine>(onofflineJson);

            NHCriteria nHCriteriaOnoff = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", onofflinetemp.RoleID);
            var obj = Singleton<NHManager>.Instance.CriteriaGet<OnOffLine>(nHCriteriaOnoff);
            Utility.Assert.NotNull(obj, () =>
            {
                Singleton<NHManager>.Instance.Update(new OnOffLine() { RoleID = onofflinetemp.RoleID, OffLineTime = DateTime.Now.ToString() });            
            }, () => { Singleton<NHManager>.Instance.Add<OnOffLine>(new OnOffLine() { RoleID = onofflinetemp.RoleID, OffLineTime = DateTime.Now.ToString() }); });
        }
    }
 }
