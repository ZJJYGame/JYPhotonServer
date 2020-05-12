using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
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
            //根据发送过来的请求获得用户名
            string username = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.UserCode.Account) as string;
            //告诉其他客户端有用户下线
            foreach (AscensionPeer temPeer in AscensionServer.Instance.PeerList)
            {
                if (string.IsNullOrEmpty(temPeer.Account) == false && temPeer != peer)
                {
                    EventData ed = new EventData((byte)EventCode.DeletePlayer);
                    Dictionary<byte, object> data2 = new Dictionary<byte, object>();
                    data2.Add((byte)ParameterCode.UserCode.Username, peer.Account);   //把下线的用户名传递给其他客户端
                    ed.Parameters = data2;
                    temPeer.SendEvent(ed, sendParameters); //发送事件
                }
            }

        }

    }
}
