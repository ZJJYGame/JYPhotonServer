using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.Model;
using Photon.SocketServer;

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
            List<PlayerData> playerDatraList = new List<PlayerData>();
            foreach (var peer in AscensionServer.ServerInstance.peerList)
            {
                if (string.IsNullOrEmpty(peer.username) == false)
                {
                    PlayerData playerData = new PlayerData();
                    playerData.Username = peer.username;
                    playerData.pos = new Vector3Data() { x = peer.x, y = peer.y, z = peer.z };
                    playerDatraList.Add(playerData);
                }
            }

            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.UserCode.Playerdatalist, Utility.Serialize(playerDatraList));

            foreach (var peer in AscensionServer.ServerInstance.peerList)
            {
                if (string.IsNullOrEmpty(peer.username) == false)
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
