/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 同步其他玩家处理者
*/
using System.Collections.Generic;
using AscensionProtocol;
using Photon.SocketServer;

namespace AscensionServer
{
    class SyncPlayerHandler : BaseHandler
    {
        public SyncPlayerHandler()
        {
            opCode = OperationCode.SyncPlayer;
        }
        //获取其他客户端相对应的用户名请求的处理代码
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            //取得所有已经登陆（在线玩家）的用户名
            List<string> userAccountList = new List<string>();

            foreach (AscensionPeer tempPeer in AscensionServer.Instance.PeerList)
            {
                //如果连接过来的客户端已经登陆了有用户名了并且这个客户端不是当前的客户端
                if (string.IsNullOrEmpty(tempPeer.Account) == false && tempPeer != peer)
                {
                    //把这些客户端的Usernam添加到集合里面
                    userAccountList.Add(tempPeer.Account);
                }
            }
            string usernameListString = Utility.ToJson(userAccountList);
            //给客户端响应
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            //data.Add((byte)ParameterCode.UserCode.Usernamelist, usernameListString);
            //data.Add((byte)ObjectParameterCode.RoleNameList, usernameListString);
            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            response.Parameters = data;
            peer.SendOperationResponse(response, sendParameters);

            //告诉其他客户端有新的客户端加入
            foreach (AscensionPeer temPeer in AscensionServer.Instance.PeerList)
            {
                if (string.IsNullOrEmpty(temPeer.Account) == false &&temPeer !=peer)
                {
                    EventData ed = new EventData((byte)EventCode.NewPlayer);
                    Dictionary<byte, object> data2 = new Dictionary<byte, object>();
                    data2.Add((byte)ParameterCode.Username, peer.Account);   //把新进来的用户名传递给其他客户端
                    //AscensionServer.log.Info(">>>>>>>>>>>>>>  " + peer.username);
                    ed.Parameters = data2;
                    temPeer.SendEvent(ed,sendParameters); //发送事件
                }
            }
        }
    }
}
