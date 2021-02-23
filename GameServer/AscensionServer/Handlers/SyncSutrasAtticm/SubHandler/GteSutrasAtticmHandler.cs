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
namespace AscensionServer
{
    public class GteSutrasAtticmHandler : SyncSutrasAtticmSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string schoolJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.School));

            var schoolObj = Utility.Json.ToObject<SchoolDTO>(schoolJson);
            SutrasAtticDTO sutrasatticObj = new SutrasAtticDTO();
            NHCriteria nHCriteriaSutrasAttic = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", schoolObj.SutrasAtticID);

            var sutrasatticTemp = NHibernateQuerier.CriteriaSelect<SutrasAttic>(nHCriteriaSutrasAttic);
            var content = RedisHelper.Hash.HashExist("SutrasAttic", sutrasatticTemp.ID.ToString());

            if (!content)
            {
                if (sutrasatticTemp!=null)
                {
                    sutrasatticObj.ID = sutrasatticTemp.ID;
                    sutrasatticObj.SutrasAmountDict = Utility.Json.ToObject<Dictionary<int, int>>(sutrasatticTemp.SutrasAmountDict);
                    sutrasatticObj.SutrasRedeemedDictl = Utility.Json.ToObject<Dictionary<int, int>>(sutrasatticTemp.SutrasRedeemedDictl);
                    SetResponseParamters(() =>
                    {
                        subResponseParameters.Add((byte)ParameterCode.SutrasAtticm, Utility.Json.ToJson(sutrasatticObj));
                        operationResponse.ReturnCode = (byte)ReturnCode.Success;
                    });
                }
                else
                {
                    SetResponseParamters(() =>
                    {
                        operationResponse.ReturnCode = (byte)ReturnCode.Fail;
                    });
                }
            }
            else
            {
                var contentSutrasAttic = RedisHelper.Hash.HashGet<SutrasAtticDTO>("Treasureattic", sutrasatticTemp.ID.ToString());
                SetResponseParamters(() =>
                {
                    subResponseParameters.Add((byte)ParameterCode.SutrasAtticm, Utility.Json.ToJson(contentSutrasAttic));
                    operationResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }

            return operationResponse;
        }
    }
}


