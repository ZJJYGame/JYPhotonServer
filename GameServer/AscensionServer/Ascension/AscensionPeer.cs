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
using Cosmos;
namespace AscensionServer
{
    //管理跟客户端的链接的
    public class AscensionPeer : ClientPeer
    {
        /// <summary>
        /// 保存当前用户登录的信息与状态
        /// </summary>
        PeerCache peerCache;
        public PeerCache PeerCache { get { return peerCache; } set { peerCache = value; } }
        //public float x, y, z;
        public RoleTransformDTO RoleTransform { get; set; }
        public string RoleTransformJson { get; set; }
        /// <summary>
        /// Peer的UUID
        /// </summary>
        public string PeerGUID { get; set; }
        public AscensionPeer(InitRequest initRequest) : base(initRequest)
        {
            peerCache = new PeerCache();
            RoleTransform = new RoleTransformDTO();
        }
        //处理客户端断开连接的后续工作
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            EventData ed = new EventData((byte)EventCode.DeletePlayer);
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ObjectParameterCode.User,peerCache);
            ed.Parameters = data;
            if (this.PeerCache.Account != null)
            {
                if (AscensionServer.Instance.HasOnlineID(this) != 0)
                {
                    RecordOnOffLine(AscensionServer.Instance.HasOnlineID(this));
                }
                AscensionServer.Instance.Offline(this);
            }

            AscensionServer.Instance.Logoff(this);
            foreach (AscensionPeer tmpPeer in AscensionServer.Instance.LoggedPeerCache.Dict.Values)
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

            //this.User.Account = user.Account;
            //this.User.UUID = user.UUID;
            //this.User.Password = user.Password;

            this.PeerCache.Account = user.Account;
            this.PeerCache.UUID= user.UUID;
            this.PeerCache.Password= user.Password;
        }
        public void Logoff()
        {
            this.PeerCache.Account =null;
            this.PeerCache.UUID = null;
            this.PeerCache.Password =null;
            //this.User.Account = null;
            //this.User.UUID = null;
            //this.User.Password = null;
        }
        public override string ToString()
        {
            return "######Account : " + PeerCache.Account + " ; Password : " + PeerCache.Password + "; UUID : " + PeerCache.UUID+"######";
        }
        //记录客户端离线时间
        protected void RecordOnOffLine(int roleid)
        {
            AscensionServer._Log.Info("同步离线时间成功");
            OffLineTimeDTO offLineTime = new OffLineTimeDTO() { RoleID = roleid };
            string onofflineJson = Utility.Json.ToJson(offLineTime);
            //////
            var onofflinetemp = Utility.Json.ToObject<OffLineTime>(onofflineJson);

            NHCriteria nHCriteriaOnoff = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", onofflinetemp.RoleID);
            var obj = Singleton<NHManager>.Instance.CriteriaGet<OffLineTime>(nHCriteriaOnoff);
            Utility.Assert.NotNull(obj, () =>
            {
                Singleton<NHManager>.Instance.Update(new OffLineTime() { RoleID = onofflinetemp.RoleID, OffTime = DateTime.Now.ToString() });            
            }, () => { Singleton<NHManager>.Instance.Add<OffLineTime>(new OffLineTime() { RoleID = onofflinetemp.RoleID, OffTime = DateTime.Now.ToString() }); });
        }
    }
 }
