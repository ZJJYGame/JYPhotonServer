using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
namespace AscensionServer
{
    public class Team : ITeam
    {
        Dictionary<string, PeerBase> peer = new Dictionary<string, PeerBase>();
        public void Dispose()
        {
            peer.Clear();
        }
    }
}
