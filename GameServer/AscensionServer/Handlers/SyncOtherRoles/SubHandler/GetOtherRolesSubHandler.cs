using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionProtocol.VO;
using AscensionProtocol.BO;
using Cosmos;
namespace AscensionServer
{
    public class GetOtherRolesSubHandler : SyncOtherRolesSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }
        List<RoleDataVO> roleDataList = new List<RoleDataVO>();
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {

            var dict = ParseSubDict(operationRequest);


            //取得所有已经登陆（在线玩家）的用户名
            //List<string> userAccountList = new List<string>();
            //foreach (AscensionPeer tempPeer in AscensionServer.Instance.LoggedPeer)
            //{
            //    //如果连接过来的客户端已经登陆了有用户名了并且这个客户端不是当前的客户端
            //    if (string.IsNullOrEmpty(tempPeer.PeerCache.Account) == false && tempPeer != peer)
            //    {
            //        //把这些客户端的Usernam添加到集合里面
            //        userAccountList.Add(tempPeer.PeerCache.Account);
            //    }
            //}
            var loggedPeers = AscensionServer.Instance.LoggedPeerCache.GetValuesHashSet();
            loggedPeers.Remove(peer);
            List<string> userAccountList = new List<string>(loggedPeers.Count);
            foreach (AscensionPeer tmpPeer in loggedPeers) 
            {
                userAccountList.Add(tmpPeer.PeerCache.Account);
            }
            string usernameListString = Utility.Json.ToJson(userAccountList);
            Owner.ResponseData.Clear();
            //给客户端响应
            SetResponseData(()=> {

            });
            Owner.OpResponse.OperationCode = operationRequest.OperationCode;
            Owner.OpResponse.Parameters = Owner.ResponseData;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            EventData ed = new EventData((byte)EventCode.NewPlayer);
            Owner.ResponseData.Add((byte)ParameterCode.Account, peer.PeerCache.Account);   //把新进来的用户名传递给其他客户端
            ed.Parameters = Owner.ResponseData;
            //告诉其他客户端有新的客户端加入
            foreach (AscensionPeer temPeer in AscensionServer.Instance.LoggedPeer)
            {
                if (string.IsNullOrEmpty(temPeer.PeerCache.Account) == false && temPeer != peer)
                {
                    temPeer.SendEvent(ed, sendParameters); //发送事件
                }
            }
        }
    }
}
