using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Cosmos;
namespace AscensionServer
{
    public class RoomEntity : IReference, IKeyValue<int, IPeerAgent>
    {
        #region Properties
        public int RoomId { get; private set; }
        /// <summary>
        /// 当前房间对象是否可用
        /// </summary>
        public bool Available { get; private set; }
        /// <summary>
        /// 广播消息事件委托；
        /// </summary>
        public event Action<byte, Dictionary<byte,object>> BroadcastBattleEvent
        {
            add { broadcastBattleEvent += value; }
            remove
            {
                try { broadcastBattleEvent -= value; }
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
        protected ConcurrentDictionary<int, IPeerAgent> peerDict 
            = new ConcurrentDictionary<int, IPeerAgent>();
        protected Action<byte, Dictionary<byte,object>> broadcastBattleEvent;
        protected object battleResultdata;
        #endregion

        #region Methods
        /// <summary>
        /// 初始化房间；
        /// 分配ID给当前房间
        /// </summary>
        /// <param name="roomId">分配的房间ID</param>
        public virtual void OnInit(int roomId)
        {
            Available = true;
            this.RoomId = roomId;
        }
        public virtual void Clear()
        {
            peerDict.Clear();
            canCacheCmd = true;
            Available = false;
            broadcastBattleEvent = null;
        }
        public bool TryGetValue(int key, out IPeerAgent value)
        {
            return peerDict.TryGetValue(key, out value);
        }
        public bool ContainsKey(int key)
        {
            return peerDict.ContainsKey(key);
        }
        public bool TryRemove(int key)
        {
            IPeerAgent peer;
            var result = peerDict.TryRemove(key, out peer);
            if (result)
                BroadcastBattleEvent -= peer.SendEvent;
            return result;
        }
        public bool TryAdd(int key, IPeerAgent Value)
        {
            if (Value == null)
               throw new ArgumentNullException("PeerEntity is invaild ! ");
            var result = peerDict.TryAdd(key, Value);
            if (result)
                BroadcastBattleEvent += Value.SendEvent;
            return result;
        }
        public bool TryRemove(int key, out IPeerAgent peer)
        {
            var result = peerDict.TryRemove(key, out peer);
            if (result)
                BroadcastBattleEvent -= peer.SendEvent;
            return result;
        }
        public bool TryUpdate(int key, IPeerAgent newValue, IPeerAgent comparsionValue)
        {
            var result = peerDict.TryUpdate(key, newValue, comparsionValue);
            if (result)
            {
                BroadcastBattleEvent -= comparsionValue.SendEvent;
                BroadcastBattleEvent += newValue.SendEvent;
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
        public static RoomEntity Create(params IPeerAgent[] peers)
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
        public static RoomEntity Create(params  int[] sessionIds)
        {
            List<IPeerAgent> peerSet = new List<IPeerAgent>();
            var length = sessionIds.Length;
            for (int i = 0; i < length; i++)
            {
                IPeerAgent peer;
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
        protected virtual void  BroadcastEvent(byte opCode, Dictionary<byte,object> data)
        {
            var task= BroadcastEventAsync(opCode, data);
        }
        protected virtual async Task BroadcastEventAsync(byte opCode, Dictionary<byte,object> data)
        {
            await Task.Run(() => broadcastBattleEvent?.Invoke(opCode, data));
        }
        #endregion
    }
}
