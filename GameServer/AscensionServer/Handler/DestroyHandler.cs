using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
namespace AscensionServer
{
    public class DestroyHandler: BaseHandler
    {
        public DestroyHandler()
        {
            OpCode = OperationCode.DestroyOtherRole;
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var userJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.User));
            var userObj = Utility.ToObject<User>(userJson);
            ResponseData.Clear();
            EventData ed = new EventData((byte)EventCode.DeletePlayer);
            ResponseData.Add((byte)ParameterCode.RoleIDList, peer.OnlineStateDTO.Role.RoleID);   //把下线的用户名传递给其他客户端
            ed.Parameters = ResponseData;
            foreach (AscensionPeer tmpPeer in AscensionServer.Instance.ConnectedPeerHashSet )
            {
                tmpPeer.SendEvent(ed, sendParameters);
            }
        }
    }
}
