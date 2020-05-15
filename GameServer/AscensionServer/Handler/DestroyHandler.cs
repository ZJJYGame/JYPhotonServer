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
            opCode = OperationCode.Destroy;
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            #region Legacy
            ////根据发送过来的请求获得用户名
            //string username = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.UserCode.Account) as string;
            ////告诉其他客户端有用户下线
            //foreach (AscensionPeer temPeer in AscensionServer.Instance.PeerList)
            //{
            //    if (string.IsNullOrEmpty(temPeer.Account) == false && temPeer != peer)
            //    {
            //        EventData ed = new EventData((byte)EventCode.DeletePlayer);
            //        Dictionary<byte, object> data = new Dictionary<byte, object>();
            //        data.Add((byte)ObjectParameterCode.RoleID, peer.OnlineStateDTO.Role.RoleId);   //把下线的用户名传递给其他客户端
            //        ed.Parameters = data;
            //        temPeer.SendEvent(ed, sendParameters); //发送事件
            //    }
            //}
            #endregion
            var userJson = Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.User) as string;

            var userObj = Utility.ToObject<User>(userJson);
            AscensionServer.Instance.DeregisterPeerInScene(peer);
            EventData ed = new EventData((byte)EventCode.DeletePlayer);
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.RoleIDList, peer.OnlineStateDTO.Role.RoleId);   //把下线的用户名传递给其他客户端
            ed.Parameters = data;
            foreach (AscensionPeer tmpPeer in AscensionServer.Instance.ConnectedPeerHashSet )
            {
                tmpPeer.SendEvent(ed, sendParameters);
            }
        }
    }
}
