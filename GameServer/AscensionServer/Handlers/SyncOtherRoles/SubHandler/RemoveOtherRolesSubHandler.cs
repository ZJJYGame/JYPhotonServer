using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    public class RemoveOtherRolesSubHandler : SyncOtherRolesSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = AscensionProtocol.SubOperationCode.Remove;
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var userJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.User));
            var userObj = Utility.Json.ToObject<User>(userJson);
            Owner. ResponseData.Clear();
            EventData ed = new EventData((byte)EventCode.DeletePlayer);
            Owner. ResponseData.Add((byte)ParameterCode.RoleIDList, peer.PeerCache.RoleID);   //把下线的用户名传递给其他客户端
            ed.Parameters = Owner. ResponseData;
            foreach (AscensionPeer tmpPeer in AscensionServer.Instance.ConnectedPeerHashSet)
            {
                tmpPeer.SendEvent(ed, sendParameters);
            }
        }
    }
}
