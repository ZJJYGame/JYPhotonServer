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
    public class TeamManager : ConcurrentSingleton<TeamManager>
    {
        CancellationToken cancelToken = new CancellationToken();
        ConcurrentDictionary<int, TeamCache> teamDict = new ConcurrentDictionary<int, TeamCache>();
        ConcurrentVariable<HashSet<int>> matchingQueue = new ConcurrentVariable<HashSet<int>>();
        public TeamCache CreateTeam(int createrID)
        {
            if (teamDict.ContainsKey(createrID))
                return null;
            var tc = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<TeamCache>();
            //tc.InitTeam(createrID,);
            return tc;
        }
        public bool LeaveMatchQueue(int peerID)
        {
            return matchingQueue.Data.Remove(peerID);
        }
        /// <summary>
        ///加入随机匹配队列
        /// </summary>
        public bool JoinMatchQueue(int peerID)
        {
            return matchingQueue.Data.Add(peerID);
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
                    if (teamDict.Count <= 0 || matchingQueue.Data.Count <= 0)
                        return;
                    HashSet<int> removeIDSet = new HashSet<int>();
                    int teamID = -1;
                    foreach (var team in teamDict.Values)
                    {
                        if (!team.IsFull)
                            teamID=team.TeamID;
                    }
                    await Task.Run(() =>
                    {
                        foreach (var id in matchingQueue.Data)
                        {
                            if (teamID == -1)
                                continue;
                            TeamCache tc;
                            teamDict.TryGetValue(teamID, out tc);
                            tc.JoinTeam(id);
                            removeIDSet.Add(id);
                        }
                        foreach (var id in removeIDSet)
                        {
                            matchingQueue.Data.Remove(id);
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
        public bool JoinTeam(int teamID, int peerID)
        {
            TeamCache gc;
            if (!teamDict.TryGetValue(teamID, out gc))
                return false;
            else
                return gc.JoinTeam(peerID);
        }

        /// <summary>
        /// 异步获取留有空位的小队ID；
        /// 若无空余，则返回-1；
        /// </summary>
        /// <returns>空虚小队的ID</returns>
    }
}
