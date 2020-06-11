using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionCalculator;
namespace AscensionServer
{
    public class Room : IRoom
    {
        /// <summary>
        /// 字典，roleID作为Key，peer作为主键
        /// </summary>
        Dictionary<int, AscensionPeer> peerDict = new Dictionary<int, AscensionPeer>();
        Dictionary<Guid, Team> teamDict = new Dictionary<Guid, Team>();
        public void Dispose()
        {
            peerDict.Clear();
        }
        /// <summary>
        /// 战斗指令集
        /// </summary>
        Dictionary<int, List<BattleCode>> instructionSet = new Dictionary<int, List<BattleCode>>();
        public void AddPeer(AscensionPeer peer)
        {
            if (!peerDict.ContainsKey(peer.OnlineStateDTO.RoleID))
            {
                peerDict.Add(peer.OnlineStateDTO.RoleID, peer);
            }
        }
        public void RemovePeer(AscensionPeer peer)
        {
            if (peerDict.ContainsKey(peer.OnlineStateDTO.RoleID))
            {
                peerDict.Remove(peer.OnlineStateDTO.RoleID);
            }
        }
    }
}
