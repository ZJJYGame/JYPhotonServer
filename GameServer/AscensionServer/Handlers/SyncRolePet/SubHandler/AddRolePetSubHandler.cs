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
    public class AddRolePetSubHandler : SyncRolePetSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;
        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            //var dict = operationRequest.Parameters;

            //string rpJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RolePet));
            //string pJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Pet));
            //string psJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.PetStatus));
            //string ppJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.PetPtitude));
            //var rolepetObj = Utility.Json.ToObject<RolePet>(rpJson);
            //var petObj = Utility.Json.ToObject<Pet>(pJson);
            //Utility.Debug.LogError("添加的角色宠物" + pJson);
            //var petstatusObj = Utility.Json.ToObject<PetStatus>(psJson);
            //var petPtitudeObj = Utility.Json.ToObject<PetaPtitudeDTO>(ppJson);
            //NHCriteria nHCriteriaroleID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", rolepetObj.RoleID);
            //var rolepet = NHibernateQuerier.CriteriaSelect<RolePet>(nHCriteriaroleID);
            //Dictionary<int, int> petDict;
            //List<string> petDoList = new List<string>();

            //if (rolepet != null)
            //{
            //    if (!string.IsNullOrEmpty(rolepet.PetIDDict))
            //    {
            //        petDict = new Dictionary<int, int>();
            //        petDict = Utility.Json.ToObject<Dictionary<int, int>>(rolepet.PetIDDict);

            //        petObj = NHibernateQuerier.Insert<Pet>(petObj);
            //        petstatusObj.PetID = petObj.ID;
            //        petstatusObj = NHibernateQuerier.Insert<PetStatus>(petstatusObj);
            //        petDict.Add(petObj.ID, petObj.PetID);
            //        PetaPtitude petaPtitude = new PetaPtitude() { AttackphysicalAptitude = petPtitudeObj.AttackphysicalAptitude, HPAptitude = petPtitudeObj.HPAptitude, AttackpowerAptitude = petPtitudeObj.AttackpowerAptitude, DefendphysicalAptitude = petPtitudeObj.DefendphysicalAptitude = petPtitudeObj.DefendphysicalAptitude, AttackspeedAptitude = petPtitudeObj.AttackspeedAptitude, DefendpowerAptitude = petPtitudeObj.DefendpowerAptitude,  Petaptitudecol = petPtitudeObj.Petaptitudecol, PetaptitudeDrug = "{}", PetID = petObj.ID, SoulAptitude = petPtitudeObj.SoulAptitude };
            //        NHibernateQuerier.Insert<PetaPtitude>(petaPtitude);
            //        NHibernateQuerier.Update(new RolePet() { RoleID = rolepet.RoleID, PetIDDict = Utility.Json.ToJson(petDict) });
            //        petDoList.Add(Utility.Json.ToJson(petObj));
            //        petDoList.Add(Utility.Json.ToJson(petstatusObj));
            //        petPtitudeObj.PetID = petObj.ID;
            //        petDoList.Add(Utility.Json.ToJson(petPtitudeObj));

            //        #region redis模块
            //        RedisHelper.Hash.HashSet<PetStatus>("PetStatus", petObj.ID.ToString(), petstatusObj);
            //        RedisHelper.Hash.HashSet<PetaPtitude>("PetaPtitude", petObj.ID.ToString(), petaPtitude);
            //        RedisHelper.Hash.HashSet<Pet>("Pet", petObj.ID.ToString(), petObj);
            //        RedisHelper.Hash.HashSet<RolePet>("RolePet", rolepetObj.RoleID.ToString(), new RolePet() { RoleID = rolepet.RoleID, PetIDDict = Utility.Json.ToJson(petDict) });

            //        Utility.Debug.LogError("添加宠物成功》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》");

            //        #endregion
            //    }
            //    else
            //    {
            //        petDict = new Dictionary<int, int>();
            //        petDict = Utility.Json.ToObject<Dictionary<int, int>>(rolepet.PetIDDict);

            //        petObj = NHibernateQuerier.Insert<Pet>(petObj);
            //        petstatusObj.PetID = petObj.ID;
            //        petstatusObj = NHibernateQuerier.Insert<PetStatus>(petstatusObj);
            //        petDict.Add(petObj.ID, petObj.PetID);
            //        PetaPtitude petaPtitude = new PetaPtitude() { AttackphysicalAptitude = petPtitudeObj.AttackphysicalAptitude, HPAptitude = petPtitudeObj.HPAptitude, AttackpowerAptitude = petPtitudeObj.AttackpowerAptitude,DefendphysicalAptitude = petPtitudeObj.DefendphysicalAptitude = petPtitudeObj.DefendphysicalAptitude, AttackspeedAptitude = petPtitudeObj.AttackspeedAptitude, DefendpowerAptitude = petPtitudeObj.DefendpowerAptitude, Petaptitudecol = petPtitudeObj.Petaptitudecol, PetaptitudeDrug = "{}", PetID = petObj.ID, SoulAptitude = petPtitudeObj.SoulAptitude };
            //        NHibernateQuerier.Insert<PetaPtitude>(petaPtitude);
            //        NHibernateQuerier.Update(new RolePet() { RoleID = rolepet.RoleID, PetIDDict = Utility.Json.ToJson(petDict) });
            //        petDoList.Add(Utility.Json.ToJson(petObj));
            //        petDoList.Add(Utility.Json.ToJson(petstatusObj));
            //        petPtitudeObj.PetID = petObj.ID;
            //        petDoList.Add(Utility.Json.ToJson(petPtitudeObj));
            //        #region redis模块
            //        RedisHelper.Hash.HashSet<PetStatus>("PetStatus", petObj.ID.ToString(), petstatusObj);
            //        RedisHelper.Hash.HashSet<PetaPtitude>("PetaPtitude", petObj.ID.ToString(), petaPtitude);
            //        RedisHelper.Hash.HashSet<Pet>("Pet", petObj.ID.ToString(), petObj);
            //        RedisHelper.Hash.HashSet<RolePet>("RolePet", rolepetObj.RoleID.ToString(), new RolePet() { RoleID = rolepet.RoleID, PetIDDict = Utility.Json.ToJson(petDict) });

            //        Utility.Debug.LogError("添加宠物成功》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》");

            //        #endregion
            //    }
            //}
            //else
            //{
            //    petDict = new Dictionary<int, int>();
            //    petDict = Utility.Json.ToObject<Dictionary<int, int>>(rolepet.PetIDDict);

            //    petObj = NHibernateQuerier.Insert<Pet>(petObj);
            //    petstatusObj.PetID = petObj.ID;
            //    petstatusObj = NHibernateQuerier.Insert<PetStatus>(petstatusObj);
            //    petDict.Add(petObj.ID, petObj.PetID);
            //    PetaPtitude petaPtitude = new PetaPtitude() { AttackphysicalAptitude = petPtitudeObj.AttackphysicalAptitude, HPAptitude = petPtitudeObj.HPAptitude, AttackpowerAptitude = petPtitudeObj.AttackpowerAptitude, DefendphysicalAptitude = petPtitudeObj.DefendphysicalAptitude = petPtitudeObj.DefendphysicalAptitude, AttackspeedAptitude = petPtitudeObj.AttackspeedAptitude, DefendpowerAptitude = petPtitudeObj.DefendpowerAptitude,Petaptitudecol = petPtitudeObj.Petaptitudecol, PetaptitudeDrug = "{}", PetID = petObj.ID, SoulAptitude = petPtitudeObj.SoulAptitude };
            //    NHibernateQuerier.Insert<PetaPtitude>(petaPtitude);
            //    NHibernateQuerier.Insert(new RolePet() { RoleID = rolepet.RoleID, PetIDDict = Utility.Json.ToJson(petDict) });
            //    petDoList.Add(Utility.Json.ToJson(petObj));
            //    petDoList.Add(Utility.Json.ToJson(petstatusObj));
            //    petPtitudeObj.PetID = petObj.ID;
            //    petDoList.Add(Utility.Json.ToJson(petPtitudeObj));
            //    #region redis模块
            //    RedisHelper.Hash.HashSet<PetStatus>("PetStatus", petObj.ID.ToString(), petstatusObj);
            //    RedisHelper.Hash.HashSet<PetaPtitude>("PetaPtitude", petObj.ID.ToString(), petaPtitude);
            //    RedisHelper.Hash.HashSet<Pet>("Pet", petObj.ID.ToString(), petObj);
            //    RedisHelper.Hash.HashSet<RolePet>("RolePet", rolepetObj.RoleID.ToString(), new RolePet() { RoleID = rolepet.RoleID, PetIDDict = Utility.Json.ToJson(petDict) });

            //    Utility.Debug.LogError("添加宠物成功》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》");

            //    #endregion
            //}
            //SetResponseParamters(() =>
            //{
            //    subResponseParameters.Add((byte)ParameterCode.RolePet, Utility.Json.ToJson(petDoList));
            //    operationResponse.ReturnCode = (short)ReturnCode.Success;
            //});
            //CosmosEntry.ReferencePoolManager.Despawns(nHCriteriaroleID);
            return operationResponse;
        }
    }
}


