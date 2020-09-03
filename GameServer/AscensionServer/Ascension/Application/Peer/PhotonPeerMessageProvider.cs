using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class PhotonPeerMessageProvider : IPeerMessageProvider
    {
        AscensionPeer peer;
        public PhotonPeerMessageProvider(AscensionPeer peer)
        {
            this.peer=peer;
        }
        public byte SendEvent(object eventData)
        {
            throw new NotImplementedException();
        }
        public byte SendMessage(object message)
        {
            throw new NotImplementedException();
        }
        public byte SendOperationResponse(object operationResponse)
        {
            throw new NotImplementedException();
        }
    }
}
