﻿/*
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
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, JYClientPeer peer)
        {
            //取得所有已经登陆（在线玩家）的用户名
            List<string> usernameList = new List<string>();

            foreach (JYClientPeer tempPeer in AscensionServer.ServerInstance.peerList)
            {
                //string.IsNullOrEmpty(tempPeer.username);//如果用户名为空表示没有登陆
                //如果连接过来的客户端已经登陆了有用户名了并且这个客户端不是当前的客户端
                if (string.IsNullOrEmpty(tempPeer.username) == false && tempPeer != peer)
                {
                    //把这些客户端的Usernam添加到集合里面
                    usernameList.Add(tempPeer.username);
                    //AscensionServer.log.Info("username = >>>>>>> " + tempPeer.username);
                }
            }
            //string usernameListString = Utility.Serialize(usernameList);
            string usernameListString = Utility.ToJson(usernameList);
            //给客户端响应
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.UserCode.Usernamelist, usernameListString);
            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            response.Parameters = data;
            peer.SendOperationResponse(response, sendParameters);

            //告诉其他客户端有新的客户端加入
            foreach (JYClientPeer temPeer in AscensionServer.ServerInstance.peerList)
            {
                if (string.IsNullOrEmpty(temPeer.username) == false &&temPeer !=peer)
                {
                    EventData ed = new EventData((byte)EventCode.NewPlayer);
                    Dictionary<byte, object> data2 = new Dictionary<byte, object>();
                    data2.Add((byte)ParameterCode.UserCode.Username, peer.username);   //把新进来的用户名传递给其他客户端
                    //AscensionServer.log.Info(">>>>>>>>>>>>>>  " + peer.username);
                    ed.Parameters = data2;
                    temPeer.SendEvent(ed,sendParameters); //发送事件
                }
            }
            ////告诉其他客户端有客户端离线
            //foreach (MyClientPeer temPeer in AscensionServer.ServerInstance.peerList)
            //{
            //    if (string.IsNullOrEmpty(temPeer.username) == false && temPeer != peer)
            //    {
            //        EventData ed = new EventData((byte)EventCode.DeletePlayer);
            //        Dictionary<byte, object> data2 = new Dictionary<byte, object>();
            //        data2.Add((byte)ParameterCode.UserCode.Username, peer.username);   //把新进来的用户名传递给其他客户端

            //        ed.Parameters = data2;
            //        temPeer.SendEvent(ed, sendParameters); //发送事件
            //    }
            //}

        }
    }
}
