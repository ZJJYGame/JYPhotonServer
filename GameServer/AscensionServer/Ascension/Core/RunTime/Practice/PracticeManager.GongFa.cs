using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using RedisDotNet;
using AscensionProtocol.DTO;
using AscensionProtocol;
using AscensionServer.Model;

namespace AscensionServer
{
    public partial class PracticeManager
    {
        #region Redis模块
        /// <summary>
        /// 获得角色所有功法
        /// </summary>
        /// <param name="RoleID"></param>
        async void GetRoleGongFaS2C(int RoleID)
        {
            if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleGongfaPerfix, RoleID.ToString()).Result)
            {
                var dict = await RedisHelper.Hash.HashGetAsync<RoleGongFaDTO>(RedisKeyDefine._RoleGongfaPerfix, RoleID.ToString());
                List<CultivationMethodDTO> gongFaIdList = new List<CultivationMethodDTO>();
                if (dict.GongFaIDArray.Count != 0)
                {
                    foreach (var item in dict.GongFaIDArray)
                    {
                        if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._GongfaPerfix, item.Key.ToString()).Result)
                        {
                            gongFaIdList.Add(await RedisHelper.Hash.HashGetAsync<CultivationMethodDTO>(RedisKeyDefine._RoleGongfaPerfix, RoleID.ToString()));
                        }
                    }
                    Dictionary<byte, object> dictData = new Dictionary<byte, object>();
                    dictData.Add((byte)ParameterCode.GongFa, gongFaIdList);
                    dictData.Add((byte)ParameterCode.RoleGongFa, dict);
                    ResultSuccseS2C(RoleID,PracticeOpcode.GetRoleGongfa,dictData);
                }
                else
                    GetRoleGongFaMySql(RoleID);
            }
            else
                GetRoleGongFaMySql(RoleID);
        }
        /// <summary>
        /// 获得角色秘术模块
        /// </summary>
        /// <param name="RoleID"></param>
        async void GetRoleMiShuS2C(int RoleID)
        {
            if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleMiShuPerfix, RoleID.ToString()).Result)
            {
                var dict = await RedisHelper.Hash.HashGetAsync<RoleMiShuDTO>(RedisKeyDefine._RoleMiShuPerfix, RoleID.ToString());
                List<MiShuDTO> miShuIdList = new List<MiShuDTO>();
                if (dict.MiShuIDArray.Count != 0)
                {
                    foreach (var item in dict.MiShuIDArray)
                    {
                        if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._MiShuPerfix, item.Key.ToString()).Result)
                        {
                            miShuIdList.Add(await RedisHelper.Hash.HashGetAsync<MiShuDTO>(RedisKeyDefine._RoleMiShuPerfix, RoleID.ToString()));
                        }
                    }
                    Dictionary<byte, object> dataDict = new Dictionary<byte, object>();
                    dataDict.Add((byte)ParameterCode.MiShu, miShuIdList);
                    dataDict.Add((byte)ParameterCode.RoleMiShu, dict);
                    ResultSuccseS2C(RoleID, PracticeOpcode.GetRoleMiShu, dataDict);
                }
                else
                    GetRoleMiShuMySql(RoleID);
            }
            else
                GetRoleMiShuMySql(RoleID);
        }

        async void AddGongFaS2C(int roleid,int id)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, GongFaBook>>(out var bookDict);
            GameEntry.DataManager.TryGetValue<Dictionary<int, GongFa>>(out var gongfaDict);
            if (!bookDict.TryGetValue(id, out var book))
            {
                Utility.Debug.LogError("学习功法失败1");
                ResultFailS2C(roleid, PracticeOpcode.AddGongFa);
                return;
            }

            var roleExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RolePostfix, roleid.ToString()).Result;
            var rolegongfaExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleGongfaPerfix,roleid.ToString()).Result;
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleid);
            var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteria);
            if (rolegongfaExist && roleExist && ringServer != null)
            {
                if (!InventoryManager.VerifyIsExist(id, 1, ringServer.RingIdArray))
                {
                    Utility.Debug.LogError("学习功法失败2");
                    ResultFailS2C(roleid, PracticeOpcode.AddGongFa);
                    return;
                }
                var rolegongfa = RedisHelper.Hash.HashGetAsync<RoleGongFaDTO>(RedisKeyDefine._RoleGongfaPerfix, roleid.ToString()).Result;
                var role = RedisHelper.Hash.HashGetAsync<Role>(RedisKeyDefine._RolePostfix, roleid.ToString()).Result;

                if (rolegongfa != null && role != null)
                {
                    if (book.NeedRoleLeve > role.RoleLevel)
                    {
                        Utility.Debug.LogError("学习功法失败3");
                        ResultFailS2C(roleid, PracticeOpcode.AddGongFa);
                        return;
                    }

                    if (rolegongfa.GongFaIDArray.Count != 0)
                    {
                        if (rolegongfa.GongFaIDArray.ContainsKey(book.GongfaID) && !rolegongfa.GongFaIDArray.ContainsKey(book.NeedGongfaID))
                        {
                            Utility.Debug.LogError("学习功法失败4");
                            ResultFailS2C(roleid, PracticeOpcode.AddGongFa);
                            return;
                        }
                        else
                        {
                            CultivationMethodDTO cultivationMethodDTO = new CultivationMethodDTO();
                            cultivationMethodDTO.CultivationMethodID = book.GongfaID;
                            cultivationMethodDTO.CultivationMethodLevel = (short)book.NeedRoleLeve;
                            for (int i = 0; i < gongfaDict[book.GongfaID].Skill_One.Count; i++)
                            {
                                if (role.RoleLevel >= gongfaDict[book.GongfaID].Skill_One_At_Level[i])
                                {
                                    cultivationMethodDTO.CultivationMethodLevelSkillArray.Add(gongfaDict[book.GongfaID].Skill_One[i]);
                                }
                            }
                            var gongfaObj = NHibernateQuerier.Insert(ChangeGongFa(cultivationMethodDTO));
                            rolegongfa.GongFaIDArray.Add(gongfaObj.ID, book.GongfaID);
                            cultivationMethodDTO.ID = gongfaObj.ID;

                            Dictionary<byte, object> dict = new Dictionary<byte, object>();
                            dict.Add((byte)ParameterCode.RoleGongFa, rolegongfa);
                            dict.Add((byte)ParameterCode.GongFa, cultivationMethodDTO);
                            dict.Add((byte)ParameterCode.RoleStatus, new RoleStatus());
                            dict.Add((byte)ParameterCode.Role, role);
                            ResultSuccseS2C(roleid, PracticeOpcode.AddGongFa, dict);

                            InventoryManager.Remove(roleid, id);
                            await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._GongfaPerfix, gongfaObj.ID.ToString(), ChangeGongFa(gongfaObj));
                            await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleGongfaPerfix, gongfaObj.ID.ToString(), rolegongfa);
                            await NHibernateQuerier.UpdateAsync(ChangeDataType(rolegongfa));
                        }
                    }
                    else
                    {
                        CultivationMethodDTO cultivationMethodDTO = new CultivationMethodDTO();
                        cultivationMethodDTO.CultivationMethodID = book.GongfaID;
                        cultivationMethodDTO.CultivationMethodLevel = 1;
                        cultivationMethodDTO.CultivationMethodLevelSkillArray.Add(gongfaDict[book.GongfaID].Skill_One[0]);
                        var gongfaObj = NHibernateQuerier.Insert(ChangeGongFa(cultivationMethodDTO));
                        rolegongfa.GongFaIDArray.Add(gongfaObj.ID, book.GongfaID);

                        role.RoleLevel = cultivationMethodDTO.CultivationMethodLevel;
                        cultivationMethodDTO.ID = gongfaObj.ID;


                        Dictionary<byte, object> dict = new Dictionary<byte, object>();
                        dict.Add((byte)ParameterCode.RoleGongFa, rolegongfa);
                        dict.Add((byte)ParameterCode.GongFa, cultivationMethodDTO);
                        dict.Add((byte)ParameterCode.RoleStatus, new RoleStatus());
                        dict.Add((byte)ParameterCode.Role, role);
                        ResultSuccseS2C(roleid, PracticeOpcode.AddGongFa, dict);

                        InventoryManager.Remove(roleid, id);
                        await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._GongfaPerfix, gongfaObj.ID.ToString(), ChangeGongFa(gongfaObj));
                        await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleGongfaPerfix, gongfaObj.ID.ToString(), rolegongfa);
                        await RedisHelper.Hash.HashSetAsync<Role>(RedisKeyDefine._RolePostfix, gongfaObj.ID.ToString(), role);

                        await NHibernateQuerier.UpdateAsync(ChangeDataType(rolegongfa));
                        await NHibernateQuerier.UpdateAsync(role);

                    }
                }
                else
                {
                    Utility.Debug.LogError("学习功法失败4");
                    ResultFailS2C(roleid, PracticeOpcode.AddGongFa);
                }
            }
            else
            {
                Utility.Debug.LogError("学习功法失败4");
                ResultFailS2C(roleid, PracticeOpcode.AddGongFa);
            }

        }

        async void AddMiShuS2C(int roleid, int id)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, MishuBook>>(out var bookDict);
            GameEntry.DataManager.TryGetValue<Dictionary<int, MiShuData>>(out var mishuDict);

            if (!bookDict.TryGetValue(id, out var book))
            {
                ResultFailS2C(roleid, PracticeOpcode.AddMiShu);
                return;
            }
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleid);
            var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteria);
            var rolemishuExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleMiShuPerfix,roleid.ToString()).Result;
            var roleExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RolePostfix, roleid.ToString()).Result;
            if (rolemishuExist&&roleExist&& ringServer!=null)
            {
                if (!InventoryManager.VerifyIsExist(id,1, ringServer.RingIdArray))
                {
                    ResultFailS2C(roleid, PracticeOpcode.AddMiShu);
                    return;
                }

                var rolemishu = RedisHelper.Hash.HashGetAsync<RoleMiShuDTO>(RedisKeyDefine._RoleMiShuPerfix, roleid.ToString()).Result;
                var role = RedisHelper.Hash.HashGetAsync<RoleDTO>(RedisKeyDefine._RolePostfix, roleid.ToString()).Result;
                if (role!=null&& rolemishu!=null)
                {
                    if (rolemishu.MiShuIDArray.ContainsKey(bookDict[id].MishuID))
                    {
                        ResultFailS2C(roleid, PracticeOpcode.AddMiShu);
                        return;
                    }

                    if (bookDict[id].NeedRoleLevel > role.RoleLevel)
                    {
                        ResultFailS2C(roleid, PracticeOpcode.AddMiShu);
                        return;
                    }

                    var rootlist = Utility.Json.ToObject<List<int>>(role.RoleRoot);
                    if (!rootlist.Contains(book.BookProperty))
                    {
                        ResultFailS2C(roleid, PracticeOpcode.AddMiShu);
                        return;
                    }

                    var mishuData = mishuDict[bookDict[id].MishuID].mishuSkillDatas.Find(x=>x.Mishu_Floor==1);
                    MiShuDTO miShuDTO = new MiShuDTO() {  MiShuSkillArry = mishuData.Skill_Array_One, MiShuAdventureSkill = mishuData.Skill_Array_Two, MiShuLevel = (short)mishuData.Need_Level_ID, MiShuID = bookDict[id].MishuID };

                   var mishuObj= await NHibernateQuerier.InsertAsync(ChangeMiShu(miShuDTO));
                    miShuDTO.ID = mishuObj.ID;

                    rolemishu.MiShuIDArray.Add(mishuObj.ID, bookDict[id].MishuID);

                    Dictionary<byte, object> dict = new Dictionary<byte, object>();
                    dict.Add((byte)ParameterCode.RoleStatus,new RoleStatus());
                    dict.Add((byte)ParameterCode.RoleMiShu, rolemishu);
                    dict.Add((byte)ParameterCode.MiShu, miShuDTO);

                    ResultSuccseS2C(roleid,PracticeOpcode.AddMiShu, dict);

                    await  RedisHelper.Hash.HashSetAsync(RedisKeyDefine._MiShuPerfix, miShuDTO.ID.ToString(), miShuDTO);
                    await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleMiShuPerfix, roleid.ToString(), rolemishu);

                    await NHibernateQuerier.UpdateAsync(ChangeDataType(rolemishu));

                }
            }

        }
        #endregion

        #region MySql模块
        /// <summary>
        /// 获取数据库数据
        /// </summary>
        /// <param name="RoleID"></param>
        async void GetRoleGongFaMySql(int RoleID)
        {
            List<NHCriteria> nHCriterias = new List<NHCriteria>();
            List<CultivationMethodDTO> gongFaIdList = new List<CultivationMethodDTO>();
            Dictionary<byte, object> dataDict = new Dictionary<byte, object>();
            NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", RoleID);
            var role = NHibernateQuerier.CriteriaSelectAsync<RoleGongFa>(nHCriteriaRole).Result;
            if (role != null)
            {
                if (!string.IsNullOrEmpty(role.GongFaIDArray))
                {
                    foreach (var item in Utility.Json.ToObject<Dictionary<int, int>>(role.GongFaIDArray))
                    {
                        NHCriteria nHCriteriaGongFa = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", item.Key);
                        nHCriterias.Add(nHCriteriaGongFa);
                        var gongfa = NHibernateQuerier.CriteriaSelectAsync<CultivationMethod>(nHCriteriaGongFa).Result;
                        if (gongfa != null)
                        {
                            gongFaIdList.Add(ChangeGongFa(gongfa));
                        }
                    }
                    dataDict = new Dictionary<byte, object>();
                    dataDict.Add((byte)ParameterCode.GongFa, gongFaIdList);                    
                    dataDict.Add((byte)ParameterCode.RoleGongFa, ChangeDataType(role));
                    ResultSuccseS2C(RoleID, PracticeOpcode.GetRoleGongfa, dataDict);
                }
                else
                {
                    ResultFailS2C(RoleID,PracticeOpcode.GetRoleGongfa);
                }
            }
            else
            {
                ResultFailS2C(RoleID, PracticeOpcode.GetRoleGongfa);
            }

        }
        /// <summary>
        /// 获取数据库数据
        /// </summary>
        /// <param name="RoleID"></param>
        async void GetRoleMiShuMySql(int RoleID)
        {
            List<NHCriteria> nHCriterias = new List<NHCriteria>();
            List<MiShuDTO> mishuIdList = new List<MiShuDTO>();
            OperationData opData = new OperationData();
            Dictionary<byte, object> dataDict = new Dictionary<byte, object>();
            NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", RoleID);
            var role = NHibernateQuerier.CriteriaSelectAsync<RoleMiShu>(nHCriteriaRole).Result;
            if (role != null)
            {
                if (!string.IsNullOrEmpty(role.MiShuIDArray))
                {
                    foreach (var item in Utility.Json.ToObject<Dictionary<int, int>>(role.MiShuIDArray))
                    {
                        NHCriteria nHCriteriaMiShu = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", item.Key);
                        nHCriterias.Add(nHCriteriaMiShu);
                        var mishu = NHibernateQuerier.CriteriaSelectAsync<MiShu>(nHCriteriaMiShu).Result;
                        if (mishu != null)
                        {
                            mishuIdList.Add(ChangeMiShu(mishu));
                        }
                    }
                    dataDict = new Dictionary<byte, object>();
                    dataDict.Add((byte)ParameterCode.MiShu, mishuIdList);
                    dataDict.Add((byte)ParameterCode.RoleMiShu, ChangeDataType(role));
                    ResultSuccseS2C(RoleID, PracticeOpcode.GetRoleMiShu, dataDict);
                }
                else
                {
                    ResultFailS2C(RoleID, PracticeOpcode.GetRoleMiShu); 
                }
            }
            else
            {
                ResultFailS2C(RoleID, PracticeOpcode.GetRoleMiShu);
            }

        }
        #endregion
    
        #region
        CultivationMethodDTO ChangeGongFa(CultivationMethod cultivation)
        {
            CultivationMethodDTO cultivationMethodDTO = new CultivationMethodDTO();
            cultivationMethodDTO.CultivationMethodExp = cultivation.CultivationMethodExp;
            cultivationMethodDTO.CultivationMethodID = cultivation.CultivationMethodID;
            cultivationMethodDTO.ID = cultivation.ID;
            cultivationMethodDTO.CultivationMethodLevel = cultivation.CultivationMethodLevel;
            cultivationMethodDTO.CultivationMethodLevelSkillArray = Utility.Json.ToObject<List<int>>(cultivation.CultivationMethodLevelSkillArray);
            return cultivationMethodDTO;
        }

        MiShuDTO ChangeMiShu(MiShu miShu)
        {
            MiShuDTO miShuDTO = new MiShuDTO();
            miShuDTO.ID = miShu.ID;
            miShuDTO.MiShuAdventureSkill = Utility.Json.ToObject<List<int>>(miShu.MiShuAdventtureSkill);
            miShuDTO.MiShuExp = miShu.MiShuExp;
            miShuDTO.MiShuID = miShu.MiShuID;
            miShuDTO.MiShuLevel = miShu.MiShuLevel;
            miShuDTO.MiShuSkillArry = Utility.Json.ToObject<List<int>>(miShu.MiShuSkillArry);

            return miShuDTO;
        }

        MiShu ChangeMiShu(MiShuDTO miShuDTO)
        {
            MiShu miShu = new MiShu();
            miShu.ID = miShuDTO.ID;
            miShu.MiShuAdventtureSkill = Utility.Json.ToJson(miShuDTO.MiShuAdventureSkill);
            miShu.MiShuExp = miShuDTO.MiShuExp;
            miShu.MiShuID = miShuDTO.MiShuID;
            miShu.MiShuLevel = miShuDTO.MiShuLevel;
            miShu.MiShuSkillArry = Utility.Json.ToJson(miShuDTO.MiShuSkillArry);

            return miShu;
        }

        RoleGongFaDTO ChangeDataType(RoleGongFa roleGongFa)
        {
            RoleGongFaDTO roleGongFaObj = new RoleGongFaDTO();
            roleGongFaObj.RoleID = roleGongFa.RoleID;
            roleGongFaObj.GongFaIDArray = Utility.Json.ToObject<Dictionary<int ,int>>(roleGongFa.GongFaIDArray);
            return roleGongFaObj;
        }
        RoleGongFa ChangeDataType(RoleGongFaDTO roleGongFa)
        {
            RoleGongFa roleGongFaObj = new RoleGongFa();
            roleGongFaObj.RoleID = roleGongFa.RoleID;
            roleGongFaObj.GongFaIDArray = Utility.Json.ToJson(roleGongFa.GongFaIDArray);
            return roleGongFaObj;
        }

        RoleMiShuDTO ChangeDataType(RoleMiShu roleMiShu)
        {
            RoleMiShuDTO roleMiShuObj = new RoleMiShuDTO();
            roleMiShuObj.RoleID = roleMiShu.RoleID;
            roleMiShuObj.MiShuIDArray = Utility.Json.ToObject<Dictionary<int, int>>(roleMiShu.MiShuIDArray);
            return roleMiShuObj;
        }

        RoleMiShu ChangeDataType(RoleMiShuDTO roleMiShu)
        {
            RoleMiShu roleMiShuObj = new RoleMiShu();
            roleMiShuObj.RoleID = roleMiShu.RoleID;
            roleMiShuObj.MiShuIDArray = Utility.Json.ToJson(roleMiShu.MiShuIDArray);
            return roleMiShuObj;
        }

        CultivationMethod ChangeGongFa(CultivationMethodDTO cultivation)
        {
            CultivationMethod cultivationMethod = new CultivationMethod();
            cultivationMethod.CultivationMethodExp = cultivation.CultivationMethodExp;
            cultivationMethod.CultivationMethodID = cultivation.CultivationMethodID;
            cultivationMethod.ID = cultivation.ID;
            cultivationMethod.CultivationMethodLevel = cultivation.CultivationMethodLevel;
            cultivationMethod.CultivationMethodLevelSkillArray = Utility.Json.ToJson(cultivation.CultivationMethodLevelSkillArray);
            return cultivationMethod;
        }

}
        #endregion

    }


