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
            string username = Utility.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.UserCode.Account) as string;
            #region    获取用户名
            //取得所有已经登陆（在线玩家）的用户名
            // List<string> usernameList = new List<string>();

            //foreach (MyClientPeer tempPeer in AscensionServer.ServerInstance.peerList)
            //{
            //    //string.IsNullOrEmpty(tempPeer.username);//如果用户名为空表示没有登陆
            //    //如果连接过来的客户端已经登陆了有用户名了并且这个客户端不是当前的客户端
            //    if (string.IsNullOrEmpty(tempPeer.username) == false && tempPeer != peer)
            //    {
            //        usernameList.Add(tempPeer.username);
            //    }
            //}
            //string usernameListString = Utility.Serialize(usernameList);
            ////给客户端响应
            //Dictionary<byte, object> data = new Dictionary<byte, object>();
            //data.Add((byte)ParameterCode.UserCode.Usernamelist, usernameListString);
            //OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            //response.Parameters = data;
            //peer.SendOperationResponse(response, sendParameters);
            #endregion

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
