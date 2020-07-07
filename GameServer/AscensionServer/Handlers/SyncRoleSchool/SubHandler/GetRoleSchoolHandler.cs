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

namespace AscensionServer
{
    public class GetRoleSchoolHandler : SyncRoleSchoolSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {


            var dict = ParseSubDict(operationRequest);
            string roleListJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleSchool));
            var roleobj = Utility.Json.ToObject<List<int>>(roleListJson);
            Dictionary<int, School> schoolDict = new Dictionary<int, School>();
            if (roleobj.Count>0)
            {
                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>..收到请求宗门的请求"+ roleListJson);

                foreach (var roleId in roleobj)
                {
                    NHCriteria nHCriteriarole = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleId);
                    var roleschoolObj = Singleton<NHManager>.Instance.CriteriaSelect<RoleSchool>(nHCriteriarole);
                    if (!string.IsNullOrEmpty(roleschoolObj.RoleJoiningSchool))
                    {
                        AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>..收到请求宗门的请求2");
                        foreach (var item in Utility.Json.ToObject<Dictionary<int, int>>(roleschoolObj.RoleJoiningSchool))
                        {
                            NHCriteria nHCriteriaSchool = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", item.Key);
                            var schoolObj = Singleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaSchool);
                            schoolDict.Add(roleId, schoolObj);
                            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaSchool);
                        }

                    }
                 
                    Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriarole);
                }
                SetResponseData(() =>
                {

                    SubDict.Add((byte)ParameterCode.RoleSchool, Utility.Json.ToJson(schoolDict));
                    AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>..发送加入的宗门" + Utility.Json.ToJson(schoolDict));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }
            
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);

        }
    }
}
