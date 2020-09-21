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
using RedisDotNet;

namespace AscensionServer
{
    public class UpdateRolePetSubHandler: SyncRolePetSubHandler
    {

        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }


        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string rolepet = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RolePet));

            var rolepetObj = Utility.Json.ToObject<RolePet>(rolepet);
            NHCriteria nHCriteriaRolePet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", rolepetObj.RoleID);

            var rolepets = NHibernateQuerier.CriteriaSelectAsync<RolePet>(nHCriteriaRolePet).Result;

            if (RedisHelper.Hash.HashExistAsync("RolePet", rolepetObj.RoleID.ToString()).Result&& rolepets != null)
            {
                #region Redis模块
                rolepets.PetIsBattle = rolepetObj.PetIsBattle;
               await  RedisHelper.Hash.HashSetAsync<RolePet>("RolePet", rolepets.RoleID.ToString(),rolepets);

               await  NHibernateQuerier.UpdateAsync(rolepets);

                #endregion

                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.RolePet, Utility.Json.ToJson(rolepets));
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
            GameManager.ReferencePoolManager.Despawns(nHCriteriaRolePet);
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }

    }
}
