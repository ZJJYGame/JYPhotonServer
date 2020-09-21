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
    public class UpdateSchoolSubHandler : SyncSchoolSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Update;

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string schoolJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.School));
            var schoolObj = Utility.Json.ToObject<School>(schoolJson);
            NHCriteria  nHCriteriaschool= GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", schoolObj.ID);
            var schooltemp= NHibernateQuerier.CriteriaSelect<School>(nHCriteriaschool);
            if (schooltemp!=null)
            {
                if (schoolObj.GetContributions>=0)
                {
                    schooltemp.ContributionNow += schoolObj.GetContributions;
                    schooltemp.IsSignin= schoolObj.IsSignin;
                   NHibernateQuerier.Update<School>(schooltemp);
                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.School, Utility.Json.ToJson(schooltemp));
                        Owner.OpResponseData.ReturnCode = (byte)ReturnCode.Success;
                    });
                }
                else
                {
                    SetResponseData(() =>
                    {
                        Owner.OpResponseData.ReturnCode = (byte)ReturnCode.Fail;
                    });
                }
            }
            Utility.Debug.LogInfo("更新后的宗门信息" + Utility.Json.ToJson(schooltemp));
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaschool);
        }
    }
}
