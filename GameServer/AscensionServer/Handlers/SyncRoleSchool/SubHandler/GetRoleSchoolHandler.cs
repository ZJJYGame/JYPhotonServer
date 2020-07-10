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
            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>..收到请求宗门的请求"+ roleListJson);
            var roleobj = Utility.Json.ToObject<List<int>>(roleListJson);
            Dictionary<int, School> schoolDict = new Dictionary<int, School>();
            if (roleobj.Count>0)
            {
                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>..收到请求宗门的请求"+ roleListJson);

                foreach (var roleId in roleobj)
                {
                    NHCriteria nHCriteriarole = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleId);
                    var roleschoolObj = Singleton<NHManager>.Instance.CriteriaSelect<RoleSchool>(nHCriteriarole);
                    var verify= Singleton<NHManager>.Instance.Verify<RoleSchool>(nHCriteriarole);
                    if (verify)
                    {
                        if (roleschoolObj.RoleJoiningSchool!=null)
                        {
                            NHCriteria nHCriteriaSchool = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", roleschoolObj.RoleJoiningSchool);
                            var schoolObj = Singleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaSchool);
                            schoolDict.Add(roleId, schoolObj);
                            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>..加入的ID" + roleId + "加入的宗门" + schoolObj.ID);
                            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaSchool);
                        }else
                            schoolDict.Clear();

                    }
                    else
                    {

                        SetResponseData(() =>
                        {
                            Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
                        });
                        peer.SendOperationResponse(Owner.OpResponse, sendParameters);
                        return;
                    }
                    Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriarole);
                    
                }
             
                if (schoolDict.Count == roleobj.Count)
                {
                    SetResponseData(() =>
                    {

                        SubDict.Add((byte)ParameterCode.RoleSchool, Utility.Json.ToJson(schoolDict));

                        Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                    });
                }
                else
                {
                    SetResponseData(() =>
                    {
                        Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
                    });
                }
            }
            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>>>>>>..发送加入的宗门" + SubDict.Count);
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);

        }
    }
}
