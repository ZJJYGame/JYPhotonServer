using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
using RedisDotNet;
using StackExchange.Redis;

namespace AscensionServer
{
    public class UpdateCreatTacticalSubHandler : SyncCreatTacticalSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Update;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string tacticJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.CreatTactical));
            var tacticObj = Utility.Json.ToObject<TacticalDTO>(tacticJson);

            //被迫打断或者主动取消的执行移除暂缓集合的操作
            //var Exist = GameManager.CustomeModule<TacticalDeploymentManager>().GetRemoveTacTical(tacticObj.RoleID, out TacticalDTO tacticalDTO);

            //if (Exist)
            //{
            //    SetResponseParamters(() =>
            //    {
            //        operationResponse.ReturnCode = (short)ReturnCode.Success;
            //    });
            //}
            return operationResponse;
        }
    }
}
