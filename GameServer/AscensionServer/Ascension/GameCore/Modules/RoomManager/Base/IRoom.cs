using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    public interface IRoom:IReference
    {
        void InitRoom(int roomID);
        bool PeerEnterRoom(int peerID);
        bool TeamEnterRoom(int teamID); 
        bool PeerExitRoom(int peerID);
        bool TeamExitRoom(int teamID);
        void CountDown();
    }
}
