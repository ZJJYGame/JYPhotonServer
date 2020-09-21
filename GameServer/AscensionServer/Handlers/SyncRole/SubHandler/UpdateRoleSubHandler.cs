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
    public class UpdateRoleSubHandler : SyncRoleSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }

        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string roleJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Role));


            var roleObj = Utility.Json.ToObject<RoleDTO>(roleJson);
            NHCriteria nHCriteriaRole = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            var roleTemp = NHibernateQuerier.CriteriaSelect<Role>(nHCriteriaRole);

            if (roleTemp != null)
            {
                roleTemp.RoleLevel = roleObj.RoleLevel;
              await  NHibernateQuerier.UpdateAsync(roleTemp);
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.Role, Utility.Json.ToJson(roleTemp));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
            {
                SetResponseData(() =>
                {
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }



            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
