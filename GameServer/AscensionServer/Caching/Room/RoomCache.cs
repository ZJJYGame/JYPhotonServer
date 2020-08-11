using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Cosmos;
namespace AscensionServer
{
    public class RoomCache:IReference
    {
        public int RoomID { get; set; }
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
        public void Clear()
        {
            peerDict.Clear();
            BattleInputC2S biC2S;
            do
            {
                inputCmdQueue.TryTake(out biC2S);
            } while (inputCmdQueue.Count>0);
            canCollectCmd = true;
        }
        /// <summary>
        ///进入房间 
        /// </summary>
        public bool EnterRoom(int peerID, AscensionPeer peer)
        {
            return peerDict.TryAdd(peerID, peer);
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
            //TODO 广播消息给当前房间内的玩家，表示开始新回合

            //服务器开始倒计时
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
        public void OnInitialization()
        {
            //TODO 初始化房间，分配ID
        }
        public void OnTermination()
        {
            //todo 广播清退玩家的消息，并回收此房间
        }
    }
}
