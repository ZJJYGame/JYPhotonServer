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
using AscensionServer.Threads;

namespace AscensionServer
{
    //管理跟客户端的链接的
    public class AscensionPeer : ClientPeer, IPeer
    {
        #region Properties
        /// <summary>
        /// 保存当前用户登录的信息与状态
        /// </summary>
        public PeerCache PeerCache { get; set; } = new PeerCache();
        /// <summary>
        /// 是否已经发送位置信息
        /// </summary>
        public bool IsSendedTransform { get; set; }
        /// <summary>
        /// 是否已经发送位置信息
        /// </summary>
        public bool IsUseSkill { get; set; }
        public long SessionId { get; private set; }
        public bool Available { get; private set; }
        public object Handle { get { return this; } }
        SendParameters sendParam = new SendParameters();
        EventData eventData= new EventData();
        #endregion

        #region Methods
        public AscensionPeer(InitRequest initRequest) : base(initRequest) { }

        //处理客户端断开连接的后续工作
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            EventData ed = new EventData((byte)EventCode.DeletePlayer);
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.Role, Utility.Json.ToJson(PeerCache.Role));
            data.Add((byte)ParameterCode.RoleMoveStatus, Utility.Json.ToJson(PeerCache.RoleMoveStatus));
            ed.Parameters = data;
            if (PeerCache.IsLogged)
            {
                RecordOnOffLine(PeerCache.RoleID);
            }
            Utility.Debug.LogInfo($"Client Disconnect :{ToString()}");
            var t= GameManager.OuterModule<PeerManager>().BroadcastEventAsync((byte)OperationCode.Logoff,ed, () => Logoff()); 
            Logoff();
            AscensionServer.Instance.ConnectedPeerHashSet.Remove(this);
            Utility.Debug.LogInfo("***********************  Client Disconnect    ***********************");
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
            this.PeerCache.Account = user.Account;
            this.PeerCache.UUID = user.UUID;
            this.PeerCache.Password = user.Password;
            this.PeerCache.IsLogged = true;
        }
        public void Logoff()
        {
            if (!PeerCache.IsLogged)
                return;
            this.PeerCache.IsLogged = false;
            this.PeerCache.Account = null;
            this.PeerCache.UUID = null;
            this.PeerCache.Password = null;
            this.PeerCache.RoleID = -1;
        }
        /// <summary>
        /// 外部接口的发送消息；
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void SendEventMessage(byte opCode,object userData)
        {
            var data = userData as Dictionary<byte, object>;
            eventData.Code = opCode;
            eventData.Parameters = data;
            SendEvent(eventData, sendParam);
        }
        public void Clear()
        {
            SessionId = 0;
            Available = false;
        }
        public override string ToString()
        {
            return $"######Account :{PeerCache.Account }; Password : {PeerCache.Password }; UUID : {PeerCache.UUID}######";
        }
        //记录客户端离线时间
        protected void RecordOnOffLine(int roleID)
        {
            if (roleID == -1)
            {
                Utility.Debug.LogInfo("============AscensionPeer.RecordOnOffLine() : Can't RecordOnOffLine ============");
                return;
            }
            NHCriteria nHCriteriaOnOff = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            var obj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<OffLineTime>(nHCriteriaOnOff);
            if (obj != null)
            {
                obj.OffTime = DateTime.Now.ToString();
                obj.RoleID = roleID;
                ConcurrentSingleton<NHManager>.Instance.Update(obj);
            }
            else
            {
                var offLineTimeTmp = GameManager.ReferencePoolManager.Spawn<OffLineTime>();
                offLineTimeTmp.RoleID = roleID;
                offLineTimeTmp.OffTime = DateTime.Now.ToString();
                ConcurrentSingleton<NHManager>.Instance.Insert(offLineTimeTmp);
                GameManager.ReferencePoolManager.Despawn(offLineTimeTmp);
            }
            GameManager.ReferencePoolManager.Despawns(nHCriteriaOnOff);
            Utility.Debug.LogInfo("同步离线时间成功");
        }
    }
    #endregion
}
