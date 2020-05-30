using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
namespace AscensionServer
{
    public class GetOtherRolesSubHandler : SyncOtherRolesSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            //取得所有已经登陆（在线玩家）的用户名
            List<string> userAccountList = new List<string>();
            foreach (AscensionPeer tempPeer in AscensionServer.Instance.PeerList)
            {
                //如果连接过来的客户端已经登陆了有用户名了并且这个客户端不是当前的客户端
                if (string.IsNullOrEmpty(tempPeer.User.Account) == false && tempPeer != peer)
                {
                    //把这些客户端的Usernam添加到集合里面
                    userAccountList.Add(tempPeer.User.Account);
                }
            }
            string usernameListString = Utility.ToJson(userAccountList);
            Owner.ResponseData.Clear();
            //给客户端响应
            Owner.OpResponse.OperationCode = operationRequest.OperationCode;
            Owner.OpResponse.Parameters = Owner.ResponseData;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            EventData ed = new EventData((byte)EventCode.NewPlayer);
            Owner.ResponseData.Add((byte)ParameterCode.Account, peer.User.Account);   //把新进来的用户名传递给其他客户端
            ed.Parameters = Owner.ResponseData;
            //告诉其他客户端有新的客户端加入
            foreach (AscensionPeer temPeer in AscensionServer.Instance.PeerList)
            {
                if (string.IsNullOrEmpty(temPeer.User.Account) == false && temPeer != peer)
                {
                    temPeer.SendEvent(ed, sendParameters); //发送事件
                }
            }
        }
    }
}
