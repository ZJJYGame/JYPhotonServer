﻿/*
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
        public override byte OpCode { get { return (byte)OperationCode.Logoff; } }
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            string userJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.User));
            var userObj = Utility.Json.ToObject<User>(userJson);
            responseParameters.Clear();
            //bool verified = peer.PeerCache.EqualUser(userObj) && peer.PeerCache.IsLogged == true;
            operationResponse.OperationCode = operationRequest.OperationCode;
            //if (verified)
            //{
            //    operationResponse.ReturnCode = (short)ReturnCode.Success;
            //    OpCodeEventCore.Instance.Dispatch(operationRequest.OperationCode, userObj);
            //}
            //else
            //{
            //    operationResponse.ReturnCode = (short)ReturnCode.Fail;
            //}
            return operationResponse;
        }
    }
}
