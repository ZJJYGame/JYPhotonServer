using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionProtocol;
using AscensionServer.Model;
using RedisDotNet;
using Cosmos;
using Protocol;
namespace AscensionServer
{
    public partial class SecondaryJobManager
    {
        /// <summary>
        /// 获得所有已学配方
        /// </summary>
        /// <param name="secondaryJobDTO"></param>
        /// <param name="nHCriteriarole"></param>
        public void GetRoleAlchemy(SecondaryJobDTO secondaryJobDTO, NHCriteria nHCriteriarole)
        {
            #region
            //var result = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlchemyPerfix, secondaryJobDTO.RoleID.ToString()).Result;
            //if (result)
            //{
            //    var alchemytemp = RedisHelper.Hash.HashGetAsync<AlchemyDTO>(RedisKeyDefine._AlchemyPerfix, secondaryJobDTO.RoleID.ToString()).Result;

            //    secondaryJobDTO.AlchemyDTO = alchemytemp;
            //    S2CAlchemyMessage(secondaryJobDTO.RoleID, Utility.Json.ToJson(secondaryJobDTO), ReturnCode.Success);
            //}
            //else
            //{
            //    var alchemy = NHibernateQuerier.CriteriaSelect<Alchemy>(nHCriteriarole);

            //    if (alchemy != null)
            //    {
            //        var alchemyObj = CosmosEntry.ReferencePoolManager.Spawn<AlchemyDTO>();
            //        alchemyObj.RoleID = alchemy.RoleID;
            //        alchemyObj.JobLevelExp = alchemy.JobLevelExp;
            //        alchemyObj.Recipe_Array = Utility.Json.ToObject<HashSet<int>>(alchemy.Recipe_Array);
            //        alchemyObj.JobLevel = alchemy.JobLevel;

            //        secondaryJobDTO.AlchemyDTO = alchemyObj;
            //        S2CAlchemyMessage(secondaryJobDTO.RoleID, Utility.Json.ToJson(secondaryJobDTO), ReturnCode.Success);
            //    }
            //    else
            //    {
            //        S2CAlchemyMessage(secondaryJobDTO.RoleID, "数据库没找到", ReturnCode.Fail);
            //    }
            //}

            #endregion

        }
        /// <summary>
        /// 学习新配方
        /// </summary>
        /// <param name="secondaryJobDTO"></param>
        /// <param name="nHCriteriarole"></param>
        public async void UpdateRoleAlchemy(SecondaryJobDTO secondaryJobDTO, NHCriteria nHCriteriarole)
        {
            #region
            //var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteriarole);
            //var nHCriteriaRingID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ringServer.RingIdArray);

            //if (InventoryManager.VerifyIsExist(secondaryJobDTO.UseItemID, nHCriteriaRingID))
            //{
            //    var alchemy = NHibernateQuerier.CriteriaSelect<Alchemy>(nHCriteriarole);
            //    if (alchemy != null)
            //    {
            //        var recipe = Utility.Json.ToObject<List<int>>(alchemy.Recipe_Array);
            //        if (recipe.Contains(secondaryJobDTO.UseItemID))
            //        {
            //            recipe.Add(secondaryJobDTO.UseItemID);
            //            alchemy.Recipe_Array = Utility.Json.ToJson(recipe);
            //            await NHibernateQuerier.UpdateAsync(alchemy);

            //            var alchemytemp = CosmosEntry.ReferencePoolManager.Spawn<AlchemyDTO>();
            //            alchemytemp.RoleID = alchemy.RoleID;
            //            alchemytemp.JobLevel = alchemy.JobLevel;
            //            alchemytemp.JobLevelExp = alchemy.JobLevelExp;
            //            alchemytemp.Recipe_Array = Utility.Json.ToObject<HashSet<int>>(alchemy.Recipe_Array);
            //            await RedisHelper.Hash.HashSetAsync<AlchemyDTO>(RedisKeyDefine._AlchemyPostfix, secondaryJobDTO.RoleID.ToString(), alchemytemp);

            //            S2CAlchemyMessage(secondaryJobDTO.RoleID, Utility.Json.ToJson(alchemytemp), ReturnCode.Success);
            //            CosmosEntry.ReferencePoolManager.Despawns(alchemytemp);
            //        }
            //        else
            //        { S2CAlchemyMessage(secondaryJobDTO.RoleID, null, ReturnCode.Fail); }
            //    }
            //    else
            //    { S2CAlchemyMessage(secondaryJobDTO.RoleID, null, ReturnCode.Fail); }
            //}
            //else
            //{ S2CAlchemyMessage(secondaryJobDTO.RoleID, null, ReturnCode.Fail); }
            #endregion
        }
        /// <summary>
        /// 合成配方
        /// </summary>
        /// <param name="secondaryJobDTO"></param>
        /// <param name="nHCriteriarole"></param>
        public void CompoundAlchemy(SecondaryJobDTO secondaryJobDTO, NHCriteria nHCriteriarole)
        {
            #region
            //var alchemy = NHibernateQuerier.CriteriaSelect<Alchemy>(nHCriteriarole);
            //if (alchemy != null)
            //{
            //    GameEntry.DataManager.TryGetValue<Dictionary<int, FormulaData>>(out var formulaDataDict);
            //    var recipe = Utility.Json.ToObject<List<int>>(alchemy.Recipe_Array);
            //    if (recipe.Contains(secondaryJobDTO.UseItemID))
            //    {
            //        formulaDataDict.TryGetValue(secondaryJobDTO.UseItemID, out var formulaData);

            //    }
            //    else
            //    {
            //        S2CAlchemyMessage(secondaryJobDTO.RoleID, null, ReturnCode.Fail);
            //    }
            //}
            //else
            //{
            //    S2CAlchemyMessage(secondaryJobDTO.RoleID, null, ReturnCode.Fail);
            //}

            #endregion
        }

        public void S2CAlchemyMessage(int roleid, string s2cMessage, ReturnCode returnCode)
        {
            OperationData opData = new OperationData();
            opData.DataMessage = s2cMessage;
            opData.OperationCode = (byte)OperationCode.SyncSecondaryJob;
            opData.ReturnCode = (byte)returnCode;
            GameEntry.RoleManager.SendMessage(roleid, opData);
        }

    }
}


