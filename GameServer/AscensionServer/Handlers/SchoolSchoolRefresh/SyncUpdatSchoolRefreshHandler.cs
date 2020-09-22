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
   public class SyncUpdatSchoolRefreshHandler: Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.UpdateSchoolRefresh; } }

        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            responseParameters.Clear();
            string schoolJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)
                ParameterCode.School));
            Utility.Debug.LogInfo("更新前的宗门信息");
            var schoolObj = Utility.Json.ToObject<School>(schoolJson);
            NHCriteria nHCriteriaschool = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", schoolObj.ID);
            var schooltemp = NHibernateQuerier.CriteriaSelect<School>(nHCriteriaschool);
            if (schooltemp != null)
            {
                if (schoolObj.GetContributions >= 0)
                {
                    schooltemp.ContributionNow += schoolObj.GetContributions;
                    schooltemp.IsSignin = schoolObj.IsSignin;
                    NHibernateQuerier.Update<School>(schooltemp);
                    opResponseData.ReturnCode = (byte)ReturnCode.Success;
                    responseParameters.Add((byte)ParameterCode.SchoolRefresh, Utility.Json.ToJson(schooltemp));
                    opResponseData.OperationCode = operationRequest.OperationCode;
                    opResponseData.Parameters = responseParameters;
                }
                else
                {
                   opResponseData.ReturnCode = (byte)ReturnCode.Fail;
                }
            }
            Utility.Debug.LogInfo("更新后的宗门信息" + Utility.Json.ToJson(schooltemp));
            GameManager.ReferencePoolManager.Despawns(nHCriteriaschool);
            return opResponseData;
        }
    }
}
