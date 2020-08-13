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
        int countDownSec = 15;
        /// <summary>
        /// 是否今儿收集指令
        /// </summary>
        bool canCollectCmd = false;
        /// <summary>
        /// 当前房间内战斗的回合数
        /// </summary>
        int roundCount = 0;
        /// <summary>
        /// peerID ,peer
        /// </summary>
        ConcurrentDictionary<int, AscensionPeer> peerDict = new ConcurrentDictionary<int, AscensionPeer>();
        ConcurrentBag<BattleInputC2S> inputCmdQueue = new ConcurrentBag<BattleInputC2S>();
        #endregion

        #region Methods
        /// <summary>
        /// 初始化房间；
        /// 分配ID给当前房间
        /// </summary>
        /// <param name="roomID">分配的房间ID</param>
        public void InitRoom(int roomID)
        {
            IsReleased = false;
            this.RoomID = roomID;
        }
        public void Clear()
        {
            peerDict.Clear();
            BattleInputC2S biC2S;
            do
            {
                inputCmdQueue.TryTake(out biC2S);
            } while (inputCmdQueue.Count > 0);
            canCollectCmd = true;
            IsReleased = true;
        }
        /// <summary>
        ///进入房间 
        /// </summary>
        public bool EnterRoom(int peerID)
        {
            var peer = RoleManager.Instance.GetPeer(peerID);
            if (peer != null)
                return peerDict.TryAdd(peerID, peer);
            else
                return false;
        }
        /// <summary>
        ///离开房间 
        /// </summary>
        public bool ExitRoom(int peerID)
        {
            AscensionPeer peer;
            return peerDict.TryRemove(peerID, out peer);
        }
        /// <summary>
        /// 缓存从客户端传来的指令
        /// </summary>
        public void HandleBattleInputCmdC2S(BattleInputC2S msgC2S)
        {
            if (!canCollectCmd)
                return;
            inputCmdQueue.Add(msgC2S);
        }
        /// <summary>
        /// 开始回合；
        /// 收集指令；
        /// </summary>
        void StartRound()
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
            AscensionServer._Log.Info("15秒倒计时结束");
        }
        void Broadcast()
        {
            foreach (var peer in peerDict.Values)
            {
                //TODO peer.SendEvent()
            }
        }
        #endregion
    }
}
