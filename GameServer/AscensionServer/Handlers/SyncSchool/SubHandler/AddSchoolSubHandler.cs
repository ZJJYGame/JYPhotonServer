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

namespace AscensionServer
{
    public class AddSchoolSubHandler : SyncSchoolSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;
        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string schoolJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.School));
            var schoolObj = Utility.Json.ToObject<School>(schoolJson);

            Utility.Debug.LogInfo(">>>>>>>加入1宗门的请求收到了"+ schoolJson);
            string treasureatticJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.TreasureAttic));
            var treasureatticObj = Utility.Json.ToObject<TreasureatticDTO>(treasureatticJson);

            string sutrasAtticJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.SutrasAtticm));
            var sutrasAtticObj = Utility.Json.ToObject<SutrasAtticDTO>(sutrasAtticJson);

            Utility.Debug.LogInfo(">>>>>>>加入2宗门的请求收到了"+ treasureatticJson);
            NHCriteria nHCriteriaSchool = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", schoolObj.ID);
            NHCriteria nHCriteriaTreasureattic = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", treasureatticObj.ID);
            NHCriteria nHCriteriasutrasAttic = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", sutrasAtticObj.ID);

            var schoolTemp = NHibernateQuerier.CriteriaSelect<School>(nHCriteriaSchool);
            var treasureatticTemp = NHibernateQuerier.CriteriaSelect<Treasureattic>(nHCriteriaTreasureattic);
            var sutrasAtticTemp = NHibernateQuerier.CriteriaSelect<SutrasAttic>(nHCriteriasutrasAttic);
            Dictionary<string, string> DTOdict = new Dictionary<string, string>();
            if (schoolTemp != null)
            {

                Utility.Debug.LogInfo(">>>>>>>加入宗门的请求收到了"+ schoolTemp.SchoolID);
                if (treasureatticTemp != null)
                {
                    treasureatticTemp.ID = treasureatticObj.ID;
                    treasureatticTemp.ItemAmountDict = Utility.Json.ToJson(treasureatticObj.ItemAmountDict);
                    NHibernateQuerier.Update(treasureatticTemp);
                }
                if (sutrasAtticTemp != null)
                    {
                    sutrasAtticTemp.ID = sutrasAtticObj.ID;
                    sutrasAtticTemp.SutrasAmountDict = Utility.Json.ToJson(sutrasAtticObj.SutrasAmountDict);
                        NHibernateQuerier.Update(sutrasAtticTemp);
                }
                    schoolTemp.SchoolID = schoolObj.SchoolID;
                schoolTemp.SchoolJob = schoolObj.SchoolJob;
                NHibernateQuerier.Update(schoolTemp);
                var schoolSendObj = NHibernateQuerier.CriteriaSelect<School>(nHCriteriaSchool);
                SetResponseParamters(() =>
                {
                    Utility.Debug.LogInfo(">>>>>>>返回加入宗门的数据" + Utility.Json.ToJson(schoolSendObj));
                    subResponseParameters.Add((byte)ParameterCode.School, Utility.Json.ToJson(schoolSendObj));
                    operationResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }
            else
                SetResponseParamters(() =>{operationResponse.ReturnCode = (byte)ReturnCode.Fail; });
            Utility.Debug.LogInfo(">>>>>>>加入宗门的请求收到了2" + schoolTemp.SchoolID);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaSchool, nHCriteriaTreasureattic, nHCriteriasutrasAttic);
            return operationResponse;
        }
    }
}
