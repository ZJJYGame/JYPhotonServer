using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using RedisDotNet;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    public class UpdatePetStatusSubHandler : SyncPetStatusSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Update;
        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string petstatusJson = Convert.ToString(Utility.GetValue(dict,(byte)ParameterCode.PetStatus));
            Utility.Debug.LogInfo("收到的宠物状态数据" + petstatusJson);

            var petstatusObj = Utility.Json.ToObject<PetStatus>(petstatusJson);
            NHCriteria nHCriteriapetstatus = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("PetID", petstatusObj.PetID);
            var petstatusTemp = NHibernateQuerier.CriteriaSelect<PetStatus>(nHCriteriapetstatus);

            if (petstatusTemp!=null)
            {

                petstatusTemp = petstatusObj;
                NHibernateQuerier.Update<PetStatus>(petstatusTemp);
                SetResponseParamters(() =>
                {
                    subResponseParameters.Add((byte)ParameterCode.PetStatus, Utility.Json.ToJson(petstatusTemp));
                    operationResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
            {
                SetResponseParamters(() =>
                {
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            GameManager.ReferencePoolManager.Despawns(nHCriteriapetstatus);
            return operationResponse;
        }
    }
}
