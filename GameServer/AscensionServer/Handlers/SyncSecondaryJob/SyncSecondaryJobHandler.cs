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
    public class SyncSecondaryJobHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncSecondaryJob; } }

        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            responseParameters.Clear();
            string secondaryJobJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)
                ParameterCode.SecondaryJob));
            var secondaryJobObj = Utility.Json.ToObject<SecondaryJobDTO>(secondaryJobJson);
            NHCriteria nHCriteriaRole = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", secondaryJobObj.RoleID);

            switch (secondaryJobObj.JobType)
            {
                case SecondaryJobDTO.SecondaryJobType.Ahclemy:


                    break;
                case SecondaryJobDTO.SecondaryJobType.Forge:
                    break;
                default:
                    break;
            }
            return operationResponse;
        }
    }
}
