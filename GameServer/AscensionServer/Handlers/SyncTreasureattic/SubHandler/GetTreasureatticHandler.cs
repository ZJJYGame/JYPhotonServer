using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using AscensionProtocol.DTO;
using Cosmos;
using RedisDotNet;
namespace AscensionServer
{
    public class GetTreasureatticHandler : SyncTreasureatticSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string treasureatticJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.TreasureAttic));
            string schoolJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.School));


            var treasureatticObj = Utility.Json.ToObject<TreasureatticDTO>(treasureatticJson);
            NHCriteria nHCriteriaTreasureattic = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", treasureatticObj.ID);

            var schoolObj = Utility.Json.ToObject<School>(schoolJson);
            var treasureatticTemp = NHibernateQuerier.CriteriaSelect<Treasureattic>(nHCriteriaTreasureattic);

            var content = RedisData.Initialize.GetData("Treasureattic");
            var contentTreasureattic = RedisData.Initialize.GetData("TreasureatticDTO");
            if (string.IsNullOrEmpty(content))
            {
                treasureatticObj.ItemAmountDict=Utility.Json.ToObject<Dictionary<int,int>>(content);
                treasureatticObj.ID = treasureatticTemp.ID;
                treasureatticObj.ItemRedeemedDict = Utility.Json.ToObject<Dictionary<int, int>>(contentTreasureattic);
            }
            else
            {
                treasureatticObj.ItemAmountDict = Utility.Json.ToObject<Dictionary<int, int>>(content);
                treasureatticObj.ID = treasureatticTemp.ID;
            }
            SetResponseParamters(() =>
            {
                Utility.Debug.LogInfo(">>>>>>>返回加入宗门的数据" + Utility.Json.ToJson(treasureatticObj));
                subResponseParameters.Add((byte)ParameterCode.TreasureAttic, Utility.Json.ToJson(treasureatticObj));
                operationResponse.ReturnCode = (byte)ReturnCode.Success;
            });

            return operationResponse;
        }
    }
}