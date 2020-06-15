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
using Cosmos;
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
            List<RoleDataDTO> roleDataList= new List<RoleDataDTO>();
            foreach (var peer in AscensionServer.Instance.LoggedPeer)
            {
                if (string.IsNullOrEmpty(peer.PeerCache.Account) == false)
                {
                    PlayerDataDTO playerData = new PlayerDataDTO();
                    playerData.Username = peer.PeerCache.Account;
                    //playerData.pos = new Vector3DataDTO() { x = peer.x, y = peer.y, z = peer.z };
                    //playerDatraList.Add(playerData);
                }
            }
            EventDataDict.Add((byte)ParameterCode.PlayerDataList, Utility.Json.ToJson(roleDataList));
            EventData ed = new EventData((byte)EventCode.SyncRoleTransform);
            ed.Parameters = EventDataDict;

            foreach (var loggedPeer in AscensionServer.Instance.LoginPeerDict.Values)
            {
                if (string.IsNullOrEmpty(loggedPeer.PeerCache.Account) == false)
                {
                    loggedPeer.SendEvent(ed, SendParameter);
                }
            }
        }

    }
}
