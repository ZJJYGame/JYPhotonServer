/*
*Author : Don
*Since 	:2020-05-11
*Description  : 登出处理
*/
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
    /// <summary>
    /// 账号登出处理者
    /// </summary>
    public class LogoffHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.Logoff;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string userJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.User));
            var userObj = Utility.Json.ToObject<User>(userJson);
           // AscensionServer._Log.Info("LogoffHandler \n 登出的账号" + userJson + ">>>>>>>>>>>>>>>>>>");
            ResponseData.Clear();
            bool verified = peer.PeerCache.EqualUser(userObj)&&peer.PeerCache.IsLogged==true;
            Utility.Debug.LogInfo("LogoffHandler \n 登出的账号" + peer.PeerCache.EqualUser(userObj) + ">>>>>>>>>>>>>>>>>>"+ peer.PeerCache.IsLogged);
            OpResponse.OperationCode = operationRequest.OperationCode;
            if (verified)
            {
                OpResponse.ReturnCode = (short)ReturnCode.Success;
                AscensionServer.Instance.RemoveFromLoggedUserCache(peer);
                //AscensionServer.Instance.Offline(peer);

                peer.Logoff();
            }
            else
            {
                OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }

            peer.SendOperationResponse(OpResponse, sendParameters);
        }
    }
}
