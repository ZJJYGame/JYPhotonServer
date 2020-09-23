using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
using RedisDotNet;
namespace AscensionServer
{
    public class UpdatePetHandler : SyncPetSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Update;
        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = ParseSubParameters(operationRequest);

            string petJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Pet));

            var petObj = Utility.Json.ToObject<Pet>(petJson);
            NHCriteria nHCriteriaPet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", petObj.ID);
            var pet = NHibernateQuerier.CriteriaSelect<Pet>(nHCriteriaPet);
            if (pet != null && RedisHelper.Hash.HashExistAsync("Pet", petObj.ID.ToString()).Result)
            {
                if (petObj.PetLevel!=0)
                {

                   pet.PetLevel += petObj.PetLevel;
                   pet.PetExp =petObj.PetExp;
                    NHibernateQuerier.Update(pet);
                  RedisHelper.Hash.HashSet<Pet>("Pet", pet.ID.ToString(), pet);
                }
                else
                {
                     pet.PetExp += petObj.PetExp;
                    NHibernateQuerier.Update(pet);
                   RedisHelper.Hash.HashSet<Pet>("Pet", pet.ID.ToString(), pet);
                }
                SetResponseParamters(() =>
                {
                    Utility.Debug.LogInfo(">>>>>>>>>>>>>>>>>>>>穿回去的宠物经验" + petJson);
                    subResponseParameters.Add((byte)ParameterCode.Pet, Utility.Json.ToJson(pet));
                    operationResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }
            else
            {
                Utility.Debug.LogInfo(">>>>>>>>>>>>>>>>>>>>传过来的宠物状态为空" + petJson);
                operationResponse.ReturnCode = (byte)ReturnCode.Fail;
            }
            GameManager.ReferencePoolManager.Despawns(nHCriteriaPet);
            return operationResponse;
        }
    }
}
