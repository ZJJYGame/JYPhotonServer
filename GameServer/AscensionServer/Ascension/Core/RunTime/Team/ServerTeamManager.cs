using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using System.IO;
using log4net.Config;
using AscensionServer.Threads;
using System.Reflection;
using ExitGames.Concurrency.Fibers;
using Cosmos;
using AscensionServer.Model;
using AscensionData;
using System.Threading;
namespace AscensionServer
{
    /// <summary>
    /// 待完善
    /// 未统一的服务器端组队功能；
    /// </summary>
    [CustomeModule]
    public partial class ServerTeamManager:Module<ServerTeamManager>
    {
        /// <summary>
        /// 玩家id 和队伍id 之间的映射
        /// </summary>
        public Dictionary<int, int> _playerIdToTeamIdDict = new Dictionary<int, int>();
        /// <summary>
        /// 队伍id 和队伍信息模型映射
        /// </summary>
        public Dictionary<int, TeamDTO> _teamTOModel = new Dictionary<int, TeamDTO>();
        /// <summary>
        /// 收集，释放解散的存在的队伍信息
        /// </summary>
        public List<int> _oldTeamList = new List<int>();

        int teamid = 1000;

        /// <summary>
        /// 判断是否是队长
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public bool IsLeader(int playerId)
        {
            if (!_playerIdToTeamIdDict.ContainsKey(playerId))
                return false;
            if (_teamTOModel[_playerIdToTeamIdDict[playerId]].LeaderId != 0)
                return true;
            return false;
        }

        /// <summary>
        /// 服务器返回给客户端的参数
        /// </summary>
        public Dictionary<byte,object> ServerToClientParams()
        {
            Dictionary<byte, object> subResponseParametersDict = new Dictionary<byte, object>();
            subResponseParametersDict.Add((byte)ParameterCode.RoleTeam, Utility.Json.ToJson(GameManager.CustomeModule<ServerTeamManager>()._teamTOModel));
            subResponseParametersDict.Add((byte)ParameterCode.Role, Utility.Json.ToJson(GameManager.CustomeModule<ServerTeamManager>()._playerIdToTeamIdDict));
            return subResponseParametersDict;
        }
    }
}
