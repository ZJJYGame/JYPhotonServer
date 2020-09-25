using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Cosmos;
namespace AscensionServer
{
    public class RoomEntity : IReference, IKeyValue<long, PeerEntity>
    {
        #region Properties
        public uint RoomId { get; private set; }
        /// <summary>
        /// 当前房间对象是否可用
        /// </summary>
        public bool Available { get; private set; }
        /// <summary>
        /// 广播消息事件委托；
        /// </summary>
        public event Action<byte, object> BroadcastBattleMessag
        {
            add { broadcastBattleMessage += value; }
            remove
            {
                try { broadcastBattleMessage -= value; }
                catch (Exception e) { Utility.Debug.LogError($"无法移除发送消息的委托:{e}"); }
            }
        }
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
        protected ConcurrentDictionary<long, PeerEntity> peerDict 
            = new ConcurrentDictionary<long, PeerEntity>();
        protected Action<byte, object> broadcastBattleMessage;
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
            Available = true;
            this.RoomId = roomId;
        }
        public virtual void Clear()
        {
            peerDict.Clear();
            canCacheCmd = true;
            Available = false;
            broadcastBattleMessage = null;
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
            var result = peerDict.TryRemove(key, out peer);
            if (result)
                BroadcastBattleMessag -= peer.SendEventMessage;
            return result;
        }
        public bool TryAdd(long key, PeerEntity Value)
        {
            if (Value == null)
               throw new ArgumentNullException("PeerEntity is invaild ! ");
            var result = peerDict.TryAdd(key, Value);
            if (result)
                BroadcastBattleMessag += Value.SendEventMessage;
            return result;
        }
        public bool TryRemove(long key, out PeerEntity peer)
        {
            var result = peerDict.TryRemove(key, out peer);
            if (result)
                BroadcastBattleMessag -= peer.SendEventMessage;
            return result;
        }
        public bool TryUpdate(long key, PeerEntity newValue, PeerEntity comparsionValue)
        {
            var result = peerDict.TryUpdate(key, newValue, comparsionValue);
            if (result)
            {
                BroadcastBattleMessag -= comparsionValue.SendEventMessage;
                BroadcastBattleMessag += newValue.SendEventMessage;
            }
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
        /// 通过peer实体生成房间实体；
        /// </summary>
        /// <param name="peers">peer的数组</param>
        /// <returns>生成的房间实体</returns>
        public static RoomEntity Create(params PeerEntity[] peers)
        {
            var length = peers.Length;
            var re = GameManager.ReferencePoolManager.Spawn<RoomEntity>();
            for (int i = 0; i < length; i++)
            {
                re.TryAdd(peers[i].SessionId, peers[i]);
            }
            return re;
        }
        /// <summary>
        /// 通过sessionId生成roomEntity;
        /// 若传入的任意sessionId无效，则房间实体生成失败，返回空；
        /// </summary>
        /// <param name="sessionIds">用户会话Id数组</param>
        /// <returns>生成的房间实体</returns>
        public static RoomEntity Create(params long[] sessionIds)
        {
            List<PeerEntity> peerSet = new List<PeerEntity>();
            var length = sessionIds.Length;
            for (int i = 0; i < length; i++)
            {
                PeerEntity peer;
                var result = GameManager.CustomeModule<PeerManager>().TryGetValue(sessionIds[i], out peer);
                if (result)
                    peerSet.Add(peer);
                else
                    return null;
            }
           return Create(peerSet.ToArray());
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
        protected virtual void BroadcastMessage(byte opCode, object data)
        {
            Task t = BroadcastMessageAsync(opCode, data);
        }
        protected virtual async Task BroadcastMessageAsync(byte opCode, object data)
        {
            await Task.Run(() => broadcastBattleMessage?.Invoke(opCode, data));
        }
        #endregion
    }
}
