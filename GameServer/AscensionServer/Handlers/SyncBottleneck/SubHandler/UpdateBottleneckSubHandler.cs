using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
using AscensionProtocol.DTO;
using RedisDotNet;

namespace AscensionServer
{
    public class UpdateBottleneckSubHandler : SyncBottleneckSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Update;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string bottleneckJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleBottleneck));
            var bottleneckObj = Utility.Json.ToObject<Bottleneck>(bottleneckJson);
            NHCriteria nHCriteriabottleneck = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", bottleneckObj.RoleID);
            var bottleneckTemp = NHibernateQuerier.CriteriaSelect<Bottleneck>(nHCriteriabottleneck);
            if (bottleneckTemp!=null)
            {
                bottleneckTemp.CraryVaule += bottleneckObj.CraryVaule;
                bottleneckTemp.BreakThroughVaule += bottleneckObj.BreakThroughVaule;
                bottleneckTemp.IsBottleneck = bottleneckObj.IsBottleneck;
                bottleneckTemp.IsDemon = bottleneckObj.IsDemon;
                bottleneckTemp.IsThunder = bottleneckObj.IsThunder;
                bottleneckTemp.RoleLevel += bottleneckObj.RoleLevel;
                NHibernateQuerier.Update<Bottleneck>(bottleneckTemp);
                SetResponseParamters(() => {
                    subResponseParameters.Add((byte)ParameterCode.RoleBottleneck, Utility.Json.ToJson(bottleneckTemp));
                    operationResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }else
                SetResponseParamters(() => {
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            return operationResponse;
        }
    }
}
