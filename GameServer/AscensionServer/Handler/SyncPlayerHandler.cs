using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, MyClientPeer peer)
        {
            //取得所有已经登陆（在线玩家）的用户名
            List<string> usernameList = new List<string>();

            foreach (MyClientPeer tempPeer in AscensionServer.ServerInstance.peerList)
            {
                //string.IsNullOrEmpty(tempPeer.username);//如果用户名为空表示没有登陆
                //如果连接过来的客户端已经登陆了有用户名了并且这个客户端不是当前的客户端
                if (string.IsNullOrEmpty(tempPeer.username) == false && tempPeer != peer)
                {
                    //把这些客户端的Usernam添加到集合里面
                    usernameList.Add(tempPeer.username);
                    AscensionServer.log.Info("username = >>>>>>> " + tempPeer.username);
                }
            }

            ////通过xml序列化进行数据传输,传输给客户端
            //StringWriter sw = new StringWriter();
            //XmlSerializer serlizer = new XmlSerializer(typeof(List<string>));
            //serlizer.Serialize(sw, usernameList);
            //sw.Close();
            //string usernameListString = sw.ToString();
            string usernameListString = Utility.Serialize(usernameList);
            AscensionServer.log.Info("usernameListString = >>>>>>> " + usernameListString);
            //给客户端响应
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.UsernameList, usernameListString);
            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            response.Parameters = data;
            peer.SendOperationResponse(response, sendParameters);

        }
    }
}
