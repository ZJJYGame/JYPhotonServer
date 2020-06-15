using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer;
using AscensionProtocol.DTO;
using AscensionProtocol;
using ExitGames.Client.Photon;
using System.Timers;
using Photon.SocketServer;
using EventData = Photon.SocketServer.EventData;
using System.Threading;

namespace AscensionServer.Threads
{
    public class SyncRoleTransformEvent : SyncEvent
    {
        public override  void  Handler(object state)
        {
            while (true)
            {
                Thread.Sleep(AscensionConst.ThreadExecuteInterval);
                AscensionServer._Log.Info("SyncRoleTransformEvent test");
                //SendPosition();
            }
        }
        public void SendPosition()
        {
            List<PlayerDataDTO> playerDatraList = new List<PlayerDataDTO>();
            foreach (var peer in AscensionServer.Instance.PeerList)
            {
                if (string.IsNullOrEmpty(peer.User.Account) == false)
                {
                    PlayerDataDTO playerData = new PlayerDataDTO();
                    playerData.Username = peer.User.Account;
                    playerData.pos = new Vector3DataDTO() { x = peer.x, y = peer.y, z = peer.z };
                    playerDatraList.Add(playerData);
                }
            }
            EventDataDict.Add((byte)ParameterCode.PlayerDataList, Utility.ToJson(playerDatraList));
            EventData ed = new EventData((byte)EventCode.SyncRoleTransform);
            foreach (var loggedPeer in AscensionServer.Instance.LoginPeerDict.Values)
            {
                if (string.IsNullOrEmpty(loggedPeer.OnlineStateDTO.Account) == false)
                {
                    ed.Parameters = EventDataDict;
                    loggedPeer.SendEvent(ed, SendParameter);
                }
            }
        }

    }
}
