using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Photon.SocketServer;
using Cosmos;
namespace AscensionServer.Threads
{
    public class SyncPositionThread
    {
        private Thread t;

        public void Run() {
            t = new Thread(UpdatePosition);
            t.IsBackground = true;// 后台运行
            t.Start();
        }
        private void UpdatePosition() {
            Thread.Sleep(5000);
            while (true)
            {
                Thread.Sleep(100);
                SendPosition();
            }
        }
        public void SendPosition() {
            List<PlayerDataDTO> playerDatraList = new List<PlayerDataDTO>();
            foreach (var peer in AscensionServer.Instance.LoggedPeer)
            {
                if (string.IsNullOrEmpty(peer.PeerCache.Account) == false)
                {
                    PlayerDataDTO playerData = new PlayerDataDTO();
                    playerData.Username = peer.PeerCache.Account;
                    //playerData.pos = new Vector3DataDTO() { x = peer.x, y = peer.y, z = peer.z };
                    playerDatraList.Add(playerData);
                }
            }

            Dictionary<byte, object> data = new Dictionary<byte, object>();
            //data.Add((byte)ParameterCode.UserCode.Playerdatalist, Utility.Serialize(playerDatraList));
            data.Add((byte)ParameterCode.PlayerDataList, Utility.Json.ToJson(playerDatraList));

            foreach (var peer in AscensionServer.Instance.LoggedPeer)
            {
                if (string.IsNullOrEmpty(peer.PeerCache. Account) == false)
                {
                    EventData ed = new EventData((byte)EventCode.SyncPosition);
                    ed.Parameters = data;
                    peer.SendEvent(ed, new SendParameters());
                }
            }
        }

        //终止线程
        public void Stop() {
            t.Abort();
        }

    }
}
