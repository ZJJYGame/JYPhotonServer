using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Cosmos;
namespace AscensionServer
{
    [CustomeModule]
    public class TeamManager : Module<TeamManager>
    {
        /// <summary>
        /// 房间ID长度
        /// </summary>
        readonly byte _IDLenth = 6;
        /// <summary>
        /// 生成房间时最小取值范围
        /// </summary>
        readonly int _MinValue = 1000000;
        /// <summary>
        /// 成功房间时最大取值范围
        /// </summary>
        readonly int _MaxValue = 99999999;
        CancellationToken cancelToken = new CancellationToken();
        ConcurrentDictionary<uint, TeamEntity> teamDict = new ConcurrentDictionary<uint, TeamEntity>();
        HashSet<uint>matchingQueue = new HashSet<uint>();
        public TeamEntity CreateTeam(uint createrID)
        {
            if (teamDict.ContainsKey(createrID))
                return null;
            var tc = GameManager.ReferencePoolManager.Spawn<TeamEntity>();
            tc.Oninit(createrID, CreateTeamID());
            return tc;
        }
        public bool LeaveMatchQueue(uint peerID)
        {
            return matchingQueue.Remove(peerID);
        }
        /// <summary>
        ///加入随机匹配队列
        /// </summary>
        public bool JoinMatchQueue(uint peerID)
        {
            return matchingQueue.Add(peerID);
        }
        public override void OnRefresh()
        {
            if (IsPause)
                return;
        }
        /// <summary>
        /// 服务端会一直运行；
        /// 异步随机匹配组队
        /// </summary>
        /// <returns>异步运行</returns>
        public async Task MatchTeam()
        {
            while (true)
            {
                if (!cancelToken.IsCancellationRequested)
                {
                    if (teamDict.Count <= 0 || matchingQueue.Count <= 0)
                        return;
                    HashSet<uint> removeIDSet = new HashSet<uint>();
                    uint teamID = 0;
                    foreach (var team in teamDict.Values)
                    {
                        if (!team.IsFull)
                            teamID = team.TeamID;
                    }
                    await Task.Run(() =>
                    {
                        foreach (var id in matchingQueue)
                        {
                            if (teamID == 0)
                                continue;
                            TeamEntity tc;
                            teamDict.TryGetValue(teamID, out tc);
                            tc.JoinTeam(id);
                            removeIDSet.Add(id);
                        }
                        foreach (var id in removeIDSet)
                        {
                            matchingQueue.Remove(id);
                        }
                    });
                }
            }
        }
        /// <summary>
        /// 加入指定的小队
        /// </summary>
        /// <param name="teamID">小队ID</param>
        /// <param name="peerID">peerID</param>
        /// <returns>是否加入成功</returns>
        public bool JoinTeam(uint teamID, uint peerID)
        {
            TeamEntity gc;
            if (!teamDict.TryGetValue(teamID, out gc))
                return false;
            else
                return gc.JoinTeam(peerID);
        }
        /// <summary>
        /// 生成队伍ID；
        /// 尾递归检测是否生成了同样的key
        /// </summary>
        /// <returns>生成后的ID</returns>
        uint CreateTeamID()
        {
            uint id = Convert.ToUInt32( Utility.Algorithm.CreateRandomInt(_MinValue, _MaxValue));
            if (!teamDict.ContainsKey(id))
                return id;
            else
                return CreateTeamID();
        }
    }
}
