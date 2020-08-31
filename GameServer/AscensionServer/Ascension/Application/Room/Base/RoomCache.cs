using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Cosmos;
namespace AscensionServer
{
    public class RoomCache : IReference
    {
        #region Properties
        public int RoomID { get; private set; }
        /// <summary>
        /// 当前房间是否被释放
        /// </summary>
        public bool IsReleased { get; private set; }

        protected int countDownSec = 15;
        /// <summary>
        /// 是否今儿收集指令
        /// </summary>
        protected bool canCollectCmd = false;
        /// <summary>
        /// 当前房间内战斗的回合数
        /// </summary>
        protected int roundCount = 0;
        /// <summary>
        /// peerID ,peer
        /// </summary>
        protected ConcurrentDictionary<int, ActorBase> peerDict = new ConcurrentDictionary<int, ActorBase>();
        #endregion

        #region Methods
        /// <summary>
        /// 初始化房间；
        /// 分配ID给当前房间
        /// </summary>
        /// <param name="roomID">分配的房间ID</param>
        public virtual void InitRoom(int roomID)
        {
            IsReleased = false;
            this.RoomID = roomID;
        }
        public virtual void Clear()
        {
            peerDict.Clear();
            canCollectCmd = true;
            IsReleased = true;
        }
        /// <summary>
        ///进入房间 
        /// </summary>
        //public bool EnterRoom(int peerID)
        //{
        //    var peer = ActorManager.Instance.TryGetValue(peerID);
        //    if (peer != null)
        //        return peerDict.TryAdd(peerID, peer);
        //    else
        //        return false;
        //}
        /// <summary>
        ///离开房间 
        /// </summary>
        public bool ExitRoom(int peerID)
        {
            ActorBase peer;
            return peerDict.TryRemove(peerID, out peer);
        }
        /// <summary>
        /// 缓存从客户端传来的指令
        /// </summary>
        public virtual void CacheInputCmdC2S(IInputCmd msgC2S)
        {
            if (!canCollectCmd)
                return;
        }
        /// <summary>
        /// 开始回合；
        /// 收集指令；
        /// </summary>
        protected void StartRound()
        {
            roundCount++;
            canCollectCmd = true;
            foreach (var peer in peerDict.Values)
            {
                //TODO 广播消息给当前房间内的玩家，表示开始新回合
                //服务器开始倒计时
            }
            CountDown();
        }
        public async void CountDown()
        {
            await Task.Delay(new TimeSpan(0, 0, 15));
            Broadcast();
            Utility.Debug.LogInfo("15秒倒计时结束");
        }
        protected void Broadcast()
        {
            foreach (var peer in peerDict.Values)
            {
                //TODO peer.SendEvent()
            }
        }
        #endregion
    }
}
