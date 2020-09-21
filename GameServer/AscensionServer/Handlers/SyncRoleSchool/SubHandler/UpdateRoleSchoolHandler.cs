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
    public class UpdateRoleSchoolHandler : SyncRoleSchoolSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string roleschoolJson = Convert.ToString(Utility.GetValue(dict,(byte)ParameterCode.RoleSchool));
            var roleschoolObj = Utility.Json.ToObject<RoleSchool>(roleschoolJson);

            NHCriteria nHCriteriaRoleschool = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleschoolObj.RoleID);
            var roleschooltmep = NHibernateQuerier.CriteriaSelect<RoleSchool>(nHCriteriaRoleschool);
            Dictionary<int, int> hatredDict = new Dictionary<int, int>();
            if (roleschooltmep!=null)
            {
                roleschooltmep.RoleJoinedSchool = roleschoolObj.RoleJoinedSchool;

                if (string .IsNullOrEmpty(roleschooltmep.RoleSchoolHatred))
                {
                    hatredDict=Utility.Json.ToObject<Dictionary<int,int>>(roleschoolObj.RoleSchoolHatred);
                    foreach (var item in Utility.Json.ToObject<Dictionary<int, int>>(roleschoolObj.RoleSchoolHatred))
                    {
                        if (hatredDict.ContainsKey(item.Key))
                        {
                            hatredDict[item.Key] += item.Value;
                        }
                        else
                            hatredDict.Add(item.Key, item.Value);
                        }
                }
                roleschooltmep.RoleSchoolHatred = Utility.Json.ToJson(hatredDict);
            }

        }
    }
}
