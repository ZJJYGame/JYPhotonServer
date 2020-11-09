using AscensionProtocol;
using Cosmos;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionServer.Threads;
using Protocol;
using RedisDotNet;
namespace AscensionServer
{
    public class RefreshSkillLayoutHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.RefreshSkillLayout; } }


        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            responseParameters.Clear();
            var roleJson= Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role));
            var refreshSkillLayoutJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters,(byte)ParameterCode.RefreshSkillLayout));
            RedisHelper.String.StringSet(RedisKeyDefine._SkillLayoutPerfix+ roleJson, refreshSkillLayoutJson);
            return operationResponse;
        }
    }
}
