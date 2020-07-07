using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;

namespace AscensionServer
{
    public class UpdateSchoolHandler : SyncSchoolSubHandler
    {
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {

            var dict = ParseSubDict(operationRequest);
            string schoolJson = Convert.ToString(Utility.GetValue(dict,(byte)ParameterCode.School));
            var schoolObj = Utility.Json.ToObject<School>(schoolJson);
            AscensionServer._Log.Info("接收到更新宗门的请求的宗门信息》》》》》》》》》》》》》》》》》》》》》》"+ schoolJson);
            NHCriteria nHCriteriaSchool = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", schoolObj.ID);
            var schoolTemp = Singleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaSchool);
            if (schoolTemp!=null)
            {
                //Singleton<NHManager>.Instance.Update<School>(new School() {ID= schoolTemp.ID,SchoolID= schoolObj.SchoolID,SchoolJob= schoolObj.SchoolJob,TreasureAtticID= schoolTemp.TreasureAtticID });
                schoolTemp.SchoolID = schoolObj.SchoolID;
                schoolTemp.SchoolJob = schoolObj.SchoolJob;
                Singleton<NHManager>.Instance.Update(schoolTemp);

                AscensionServer._Log.Info("返回的宗门信息》》》》》》》》》》》》》》》》》》》》》》"+Utility.Json.ToJson(schoolTemp));
                SetResponseData(() =>
                {

                    SubDict.Add((byte)ParameterCode.School, Utility.Json.ToJson(schoolTemp));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }
            else
            {
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
            }

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaSchool);
        }
    }
}
