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
    public class AddPetHandler : SyncPetSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = ParseSubParameters(operationRequest);

            string petJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Pet));

            var petObj = Utility.Json.ToObject<Pet>(petJson);
            NHCriteria nHCriteriaPet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("PetID", petObj.PetID);
            var PetObj = NHibernateQuerier.CriteriaSelect<Pet>(nHCriteriaPet);
            if (!string.IsNullOrEmpty(petJson))
            {
                petObj = NHibernateQuerier.Insert(petObj);
                Utility.Debug.LogInfo(">>>>>>>>>>>>>>>>>>>>>>>>>添加宠物进来了》》》》》》》》》》》》》》》");
                SetResponseParamters(() =>
                {
                    subResponseParameters.Add((byte)ParameterCode.Pet, petObj);
                    operationResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
            {
                operationResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            return operationResponse;
        }
    }
}

