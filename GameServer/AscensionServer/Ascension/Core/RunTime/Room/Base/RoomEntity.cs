using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Cosmos;
namespace AscensionServer
{
    public class RoomEntity : IReference,ISimpleKeyValue<long,PeerEntity>
    {
        #region Properties
        public uint RoomId { get; private set; }
        /// <summary>
        /// 当前房间对象是否可用
        /// </summary>
        public bool Available{ get; private set; }
        /// <summary>
        /// 倒计时秒
        /// </summary>
        protected int countDownSec = 15;
        /// <summary>
        /// 是否可收集指令
        /// </summary>
        protected bool canCacheCmd = false;
        /// <summary>
        /// 当前房间内战斗的回合数
        /// </summary>
        protected int roundCount = 0;
        protected ConcurrentDictionary<long, PeerEntity> peerDict = new ConcurrentDictionary<long, PeerEntity>();
        protected Action<byte,object> broadcastBattleMessage;
        protected object battleResultdata;
        #endregion

        #region Methods
        /// <summary>
        /// 初始化房间；
        /// 分配ID给当前房间
        /// </summary>
        /// <param name="roomId">分配的房间ID</param>
        public virtual void OnInit(uint roomId)
        {
            Available= true;
            this.RoomId = roomId;
        }
        public virtual void Clear()
        {
            peerDict.Clear();
            canCacheCmd = true;
            Available= false;
        }
        public bool TryGetValue(long key, out PeerEntity value)
        {
            return peerDict.TryGetValue(key, out value);
        }
        public bool ContainsKey(long key)
        {
            return peerDict.ContainsKey(key);
        }
        public bool TryRemove(long key)
        {
            PeerEntity peer;
            var result= peerDict.TryRemove(key, out peer);
            if (result)
            {
                try
                {
                    broadcastBattleMessage -= peer.SendEventMessage;
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError($"无法移除发送消息的委托:{peer.Handle.ToString()},{e}");
                }
            }
            return result;
        }
        public bool TryAdd(long key, PeerEntity Value)
        {
            if (Value == null)
                throw new ArgumentNullException("PeerEntity is invaild ! ");
            var result= peerDict.TryAdd(key, Value);
            if (result)
                broadcastBattleMessage += Value.SendEventMessage;
            return result;
        }
        /// <summary>
        /// 缓存从客户端传来的指令
        /// </summary>
        public virtual void CacheInputCmd(IInputData msg)
        {
            if (!canCacheCmd)
                return;
        }
        public async void CountDown()
        {
            await Task.Delay(new TimeSpan(0, 0, 15));
            Utility.Debug.LogInfo("15秒倒计时结束，开始广播战斗计算结果");
            //await BroadcastMessageAsync(null);
        }
        /// <summary>
        /// 开始回合；
        /// 收集指令；
        /// </summary>
        protected void StartRound()
        {
            roundCount++;
            canCacheCmd = true;
            CountDown();
        }

        protected virtual void BroadcastMessage(byte opCode,object data)
        {
            Task t= BroadcastMessageAsync(opCode,data);
        }
        protected virtual async Task BroadcastMessageAsync(byte opCode, object data)
        {
            await Task.Run(() => broadcastBattleMessage?.Invoke(opCode,data));
        }
        
        #endregion
    }
}
