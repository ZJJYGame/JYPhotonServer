using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisDotNet;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
using NHibernate.Criterion;
namespace AscensionServer
{
    public partial class GangsMananger
    {
        #region  Redis模塊
        /// <summary>
        /// 申请加入宗门
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="id"></param>
        void ApplyJoinAllianceS2C(int roleID, int id)
        {
            var result = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAlliancePerfix, roleID.ToString()).Result;
            var alianceExits = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlliancePerfix, id.ToString()).Result;
            if (result && alianceExits)
            {
                var roleAlliance = RedisHelper.Hash.HashGetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, roleID.ToString()).Result;
                var alliance = RedisHelper.Hash.HashGetAsync<AllianceMemberDTO>(RedisKeyDefine._AlliancePerfix, id.ToString()).Result;
                if (roleAlliance != null && alliance != null)
                {

                    if (!roleAlliance.ApplyForAlliance.Contains(id) && !alliance.ApplyforMember.Contains(roleID) && !alliance.Member.Contains(roleID))
                    {
                        roleAlliance.ApplyForAlliance.Add(id);
                        alliance.ApplyforMember.Add(roleID);
                        RoleStatusSuccessS2C(roleID, AllianceOpCode.JoinAlliance, roleAlliance);
                        //TODO更新到数据库
                    }
                    else
                    {
                        RoleStatusFailS2C(roleID, AllianceOpCode.JoinAlliance);
                    }
                }
                else
                {
                    ApplyJoinAllianceMySql(roleID, id);
                }
            }
        }
        /// <summary>
        /// 一鍵同意入宗申請
        /// </summary>
        async void ConsentApplyS2C(int roleID, int id, List<int> roleIDs)
        {
            var allianceExits = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AllianceMemberPerfix, id.ToString()).Result;
            var alliancestatusExits = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlliancePerfix, id.ToString()).Result;
            List<int> consents = new List<int>();
            if (allianceExits && alliancestatusExits)
            {
                var allianceObj = RedisHelper.Hash.HashGetAsync<AllianceMemberDTO>(RedisKeyDefine._AllianceMemberPerfix, id.ToString()).Result;
                var alliancestatusObj = RedisHelper.Hash.HashGetAsync<AllianceStatusDTO>(RedisKeyDefine._AlliancePerfix, id.ToString()).Result;
                if (allianceObj != null && alliancestatusObj != null)
                {
                    for (int i = 0; i < roleIDs.Count; i++)
                    {
                        if (allianceObj.ApplyforMember.Contains(roleIDs[i]) && (alliancestatusObj.AllianceNumberPeople + consents.Count) <= alliancestatusObj.AlliancePeopleMax)
                        {
                            var rolealliance = RedisHelper.Hash.HashGetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, roleIDs[i].ToString()).Result;
                            if (rolealliance != null)
                            {
                                rolealliance.AllianceID = id;
                                rolealliance.ApplyForAlliance.Clear();
                                rolealliance.JoinTime = DateTime.Now.ToString();
                                consents.Add(roleIDs[i]);
                                await RedisHelper.Hash.HashSetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, roleIDs[i].ToString(), rolealliance);
                                await NHibernateQuerier.UpdateAsync(ChangeDataType(rolealliance));
                            }
                        }
                    }
                    allianceObj.ApplyforMember.Except(consents);
                    allianceObj.Member.AddRange(consents);
                    alliancestatusObj.AllianceNumberPeople += consents.Count;

                    //TODO發送數據至客戶端
                    Dictionary<byte, object> dict = new Dictionary<byte, object>();
                    dict.Add((byte)ParameterCode.AllianceStatus, alliancestatusObj);

                    RoleStatusSuccessS2C(roleID,AllianceOpCode.ConsentApply, dict);

                    //更新到数据库
                    await RedisHelper.Hash.HashSetAsync<AllianceMemberDTO>(RedisKeyDefine._AllianceMemberPerfix, id.ToString(), allianceObj);
                    await RedisHelper.Hash.HashSetAsync<AllianceStatusDTO>(RedisKeyDefine._AlliancePerfix, id.ToString(), alliancestatusObj);
                    await NHibernateQuerier.UpdateAsync(ChangeDataType(allianceObj));
                    await NHibernateQuerier.UpdateAsync(ChangeDataType(alliancestatusObj));
                }
                else
                    ConsentApplyMySql(roleID, id, roleIDs);
            }
            else
                ConsentApplyMySql(roleID, id, roleIDs);

        }
        /// <summary>
        /// 一鍵拒絕
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="id"></param>
        /// <param name="roleIDs"></param>
        async void RefuseApplyS2C(int roleID, int id, List<int> roleIDs)
        {
            var allianceExits = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AllianceMemberPerfix, id.ToString()).Result;
            var alliancestatusExits = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlliancePerfix, id.ToString()).Result;
            List<int> refuses = new List<int>();
            if (allianceExits && alliancestatusExits)
            {
                var allianceObj = RedisHelper.Hash.HashGetAsync<AllianceMemberDTO>(RedisKeyDefine._AllianceMemberPerfix, id.ToString()).Result;
                var alliancestatusObj = RedisHelper.Hash.HashGetAsync<AllianceStatusDTO>(RedisKeyDefine._AlliancePerfix, id.ToString()).Result;
                if (allianceObj != null && alliancestatusObj != null)
                {
                    for (int i = 0; i < roleIDs.Count; i++)
                    {
                        if (allianceObj.ApplyforMember.Contains(roleIDs[i]))
                        {
                            var rolealliance = RedisHelper.Hash.HashGetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, roleIDs[i].ToString()).Result;
                            if (rolealliance != null)
                            {
                                if (rolealliance.ApplyForAlliance.Contains(roleIDs[i]))
                                {
                                    rolealliance.ApplyForAlliance.Remove(roleIDs[i]);
                                }
                                refuses.Add(roleIDs[i]);
                                await RedisHelper.Hash.HashSetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, roleIDs[i].ToString(), rolealliance);
                                await NHibernateQuerier.UpdateAsync(ChangeDataType(rolealliance));
                            }
                        }
                    }
                }
                else
                {
                    //TODOMySql模塊
                }
            }
            else
            {
                //TODOMySql模塊
            }
        }
        /// <summary>
        /// 獲取宗門成員數據
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="id"></param>
        async void GetAllianceMemberS2C(int roleID, int id)
        {
            var allianceExit = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AllianceMemberPerfix, id.ToString()).Result;
            if (allianceExit)
            {
                Utility.Debug.LogInfo("YZQ请求宗门成员1");
                var allianceObj = RedisHelper.Hash.HashGetAsync<AllianceMemberDTO>(RedisKeyDefine._AllianceMemberPerfix, id.ToString()).Result;
                List<RoleAllianceDTO> member = new List<RoleAllianceDTO>();
                List<RoleAllianceDTO> apply = new List<RoleAllianceDTO>();
                if (allianceObj != null)
                {
                    for (int i = 0; i < allianceObj.ApplyforMember.Count; i++)
                    {
                        if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAlliancePerfix, allianceObj.ApplyforMember[i].ToString()).Result)
                        {
                            var roleAlliance = await RedisHelper.Hash.HashGetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, allianceObj.ApplyforMember[i].ToString());
                            if (roleAlliance != null)
                            {
                                apply.Add(roleAlliance);
                            }
                        }

                    }
                    for (int i = 0; i < allianceObj.Member.Count; i++)
                    {
                        if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAlliancePerfix, allianceObj.ApplyforMember[i].ToString()).Result)
                        {
                            var roleAlliance = await RedisHelper.Hash.HashGetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, allianceObj.Member[i].ToString());
                            if (roleAlliance != null)
                            {
                                member.Add(roleAlliance);
                            }
                        }
                    }
                    Dictionary<byte, object> dict = new Dictionary<byte, object>();
                    dict.Add((byte)ParameterCode.AllianceMember, member);
                    dict.Add((byte)ParameterCode.ApplyMember, apply);
                    RoleStatusSuccessS2C(roleID, AllianceOpCode.GetAllianceMember, dict);
                }
                else
                    GetAllianceMemberMySql(roleID, id);
            }
            else
                GetAllianceMemberMySql(roleID, id);
        }
        /// <summary>
        /// 模糊搜索宗门信息
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        async void SearchAllianceS2C(int roleID,int id,string name)
        {
            List<NHCriteria> nHCriterias = new List<NHCriteria>();
            if (id <= 0)
            {
                NHCriteria nHCriteriaAllianceName = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceName", name);
                nHCriterias.Add(nHCriteriaAllianceName);
                var allianceNameObj = NHibernateQuerier.CriteriaLikeAsync<AllianceStatus>(nHCriteriaAllianceName, MatchMode.Anywhere).Result;
                if (allianceNameObj.Count == 0)
                {
                    RoleStatusFailS2C(roleID, AllianceOpCode.SearchAlliance);
                }
                else
                {
                    RoleStatusSuccessS2C(roleID, AllianceOpCode.SearchAlliance, allianceNameObj);
                }
            }
            else
            {
                NHCriteria nHCriteriaAllianceID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", id);
                nHCriterias.Add(nHCriteriaAllianceID);
                var allianceIDObj = NHibernateQuerier.CriteriaSelectAsync<AllianceStatus>(nHCriteriaAllianceID).Result;
                List<AllianceStatus> allianceStatuss = new List<AllianceStatus>();
                if (allianceIDObj == null)
                {
                    NHCriteria nHCriteriaAllianceName = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceName", name.ToString());
                    nHCriterias.Add(nHCriteriaAllianceName);
                    var allianceNameObj = NHibernateQuerier.CriteriaLikeAsync<AllianceStatus>(nHCriteriaAllianceName, MatchMode.Anywhere).Result;
                    if (allianceNameObj.Count == 0)
                    {
                        RoleStatusFailS2C(roleID, AllianceOpCode.SearchAlliance);
                    }
                    else
                    {
                        RoleStatusSuccessS2C(roleID, AllianceOpCode.SearchAlliance, allianceNameObj);
                    }
                }
                else
                {
                    allianceStatuss.Add(allianceIDObj);
                    NHCriteria nHCriteriaAllianceName = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceName", name);
                    nHCriterias.Add(nHCriteriaAllianceName);
                    var allianceNameObj = NHibernateQuerier.CriteriaLikeAsync<AllianceStatus>(nHCriteriaAllianceName, MatchMode.Anywhere).Result;
                    if (allianceNameObj.Count == 0)
                    {
                        RoleStatusSuccessS2C(roleID, AllianceOpCode.SearchAlliance, allianceStatuss);
                    }
                    else
                    {
                        if (!allianceNameObj.Equals(allianceIDObj))
                        {
                            allianceNameObj.Add(allianceIDObj);
                            RoleStatusSuccessS2C(roleID, AllianceOpCode.SearchAlliance, allianceNameObj);
                        }
                    }

                }
            }
        }
        /// <summary>
        /// 修改宗门职位
        /// </summary>
        async void CareerAdvancementS2C(int roleID,int id,int jobID)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, AllianceJobData>>(out var jobDict);

            var allianceExit = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AllianceMemberPerfix, id.ToString()).Result;
            List<RoleAllianceDTO> roleAlliances = new List<RoleAllianceDTO>();
            if (allianceExit)
            {
                var roleAlliance= RedisHelper.Hash.HashGetAsync<AllianceMemberDTO>(RedisKeyDefine._RoleAlliancePerfix, id.ToString()).Result;
                if (roleAlliance!=null)
                {
                    for (int i = 0; i < roleAlliance.Member.Count; i++)
                    {
                        var memberExit= RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AllianceMemberPerfix, roleAlliance.Member[i].ToString()).Result;

                        if (memberExit)
                        {
                            var member = RedisHelper.Hash.HashGetAsync<RoleAllianceDTO>(RedisKeyDefine._AllianceMemberPerfix, roleAlliance.Member[i].ToString()).Result;
                            if (member!=null)
                            {
                                roleAlliances.Add(member);
                            }
                        }
                    }
                }

                var num = roleAlliances.FindAll((x) => x.AllianceJob == jobID).Count;//获得当前职位的人数

                var result = jobDict.TryGetValue(jobID, out var jobData);
                if (result)
                {
                    //判断可安排职位人数是否足够，更新至数据库
                }

            }
        }
        #endregion


        #region MySql模塊
        /// <summary>
        /// 申請加入宗門
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="id"></param>
        async void ApplyJoinAllianceMySql(int roleID, int id)
        {
            NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            var roleAlliance = NHibernateQuerier.CriteriaSelect<RoleAlliance>(nHCriteriaRole);
            NHCriteria nHCriteriaAlliance = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", id);
            var alliance = NHibernateQuerier.CriteriaSelect<AllianceMember>(nHCriteriaAlliance);
            if (alliance != null && roleAlliance != null)
            {
                var roleAllianceList = Utility.Json.ToObject<List<int>>(roleAlliance.ApplyForAlliance);//角色申請的宗廟
                var applyMemberList = Utility.Json.ToObject<List<int>>(alliance.ApplyforMember);//宗門申請列表
                var memberList = Utility.Json.ToObject<List<int>>(alliance.Member);
                if (!roleAllianceList.Contains(id) && !applyMemberList.Contains(roleID) && !memberList.Contains(roleID))
                {
                    roleAllianceList.Add(id);
                    applyMemberList.Add(roleID);
                    roleAlliance.ApplyForAlliance = Utility.Json.ToJson(roleAllianceList);
                    alliance.ApplyforMember = Utility.Json.ToJson(applyMemberList);
                    RoleStatusSuccessS2C(roleID, AllianceOpCode.JoinAlliance, ChangeDataType(roleAlliance));
                    //TODO更新到数据库
                    await NHibernateQuerier.UpdateAsync(roleAlliance);
                    await NHibernateQuerier.UpdateAsync(alliance);

                    await RedisHelper.Hash.HashSetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, roleID.ToString(), ChangeDataType(roleAlliance));

                    await RedisHelper.Hash.HashSetAsync<AllianceMemberDTO>(RedisKeyDefine._AllianceMemberPerfix, roleID.ToString(), ChangeDataType(alliance));

                }
                else
                {
                    RoleStatusFailS2C(roleID, AllianceOpCode.JoinAlliance);
                }
            }
            else
                RoleStatusFailS2C(roleID, AllianceOpCode.JoinAlliance);
        }
        /// <summary>
        /// 一鍵同意申請加入宗廟
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roleIDs"></param>
        async void ConsentApplyMySql(int roleid, int id, List<int> roleIDs)
        {
            List<int> consents = new List<int>();
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", id);
            var allianceObj = NHibernateQuerier.CriteriaSelect<AllianceMember>(nHCriteria);
            var alliance = NHibernateQuerier.CriteriaSelect<AllianceStatus>(nHCriteria);
            if (allianceObj != null && alliance != null)
            {
                var applyeList = Utility.Json.ToObject<List<int>>(allianceObj.ApplyforMember);
                var memberList = Utility.Json.ToObject<List<int>>(allianceObj.Member);

                for (int i = 0; i < roleIDs.Count; i++)
                {
                    if (applyeList.Contains(roleIDs[i]) && (alliance.AllianceNumberPeople + consents.Count) <= alliance.AlliancePeopleMax)
                    {
                        NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleIDs[i]);
                        var roleAlliancej = NHibernateQuerier.CriteriaSelect<RoleAlliance>(nHCriteriaRole);
                        if (roleAlliancej != null)
                        {
                            roleAlliancej.AllianceID = id;
                            roleAlliancej.ApplyForAlliance = "[]";
                            roleAlliancej.JoinTime = DateTime.Now.ToString();
                            consents.Add(roleIDs[i]);
                            await RedisHelper.Hash.HashSetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, roleIDs[i].ToString(), ChangeDataType(roleAlliancej));
                            await NHibernateQuerier.UpdateAsync(roleAlliancej);
                        }
                    }
                    else
                        RoleStatusFailS2C(roleid, AllianceOpCode.ConsentApply);
                }

                allianceObj.ApplyforMember = Utility.Json.ToJson(applyeList.Except(consents));
                memberList.AddRange(consents);
                allianceObj.Member = Utility.Json.ToJson(memberList);
                alliance.AllianceNumberPeople += consents.Count;

                //TODO發送數據至客戶端

                //更新到数据库
                await RedisHelper.Hash.HashSetAsync<AllianceMemberDTO>(RedisKeyDefine._AllianceMemberPerfix, id.ToString(), ChangeDataType(allianceObj));
                await RedisHelper.Hash.HashSetAsync<AllianceStatus>(RedisKeyDefine._AlliancePerfix, id.ToString(), alliance);
                await NHibernateQuerier.UpdateAsync(ChangeDataType(allianceObj));
                await NHibernateQuerier.UpdateAsync(alliance);
            } else
                RoleStatusFailS2C(roleid, AllianceOpCode.ConsentApply);
        }
        /// <summary>
        /// 一鍵拒絕加入宗門
        /// </summary>
        async void RefuseApplyMySql(int roleid, int id, List<int> roleIDs)
        {
            List<int> refuses = new List<int>();
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", id);
            var allianceObj = NHibernateQuerier.CriteriaSelect<AllianceMember>(nHCriteria);
            var alliance = NHibernateQuerier.CriteriaSelect<AllianceStatus>(nHCriteria);
            if (allianceObj != null && alliance != null)
            {
                var applyeList = Utility.Json.ToObject<List<int>>(allianceObj.ApplyforMember);
                for (int i = 0; i < roleIDs.Count; i++)
                {
                    if (applyeList.Contains(roleIDs[i]))
                    {
                        NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleIDs[i]);
                        var roleAlliancej = NHibernateQuerier.CriteriaSelect<RoleAlliance>(nHCriteriaRole);
                        if (roleAlliancej != null)
                        {
                            var applys = Utility.Json.ToObject<List<int>>(roleAlliancej.ApplyForAlliance);
                            if (applys.Contains(id))
                            {
                                applys.Remove(id);
                            }
                            roleAlliancej.ApplyForAlliance = Utility.Json.ToJson(applys);
                            refuses.Add(roleIDs[i]);
                            await RedisHelper.Hash.HashSetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, roleIDs[i].ToString(), ChangeDataType(roleAlliancej));
                            await NHibernateQuerier.UpdateAsync(roleAlliancej);
                        }
                    }
                }
            } else
                RoleStatusFailS2C(roleid, AllianceOpCode.RefuseApply);
        }
        /// <summary>
        /// 獲取宗門成員數據
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="id"></param>
        async void GetAllianceMemberMySql(int roleID, int id)
        {
            Utility.Debug.LogInfo("YZQ请求宗门成员2");
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", id);
            var allianceObj = NHibernateQuerier.CriteriaSelect<AllianceMember>(nHCriteria);
            List<RoleAllianceDTO> memberlist = new List<RoleAllianceDTO>();
            List<RoleAllianceDTO> applylist = new List<RoleAllianceDTO>();
            if (allianceObj != null)
            {
                var member = Utility.Json.ToObject<List<int>>(allianceObj.Member);
                var apply = Utility.Json.ToObject<List<int>>(allianceObj.ApplyforMember);
                for (int i = 0; i < apply.Count; i++)
                {
                    NHCriteria nHCriteriaalliance = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
                    var alliance = NHibernateQuerier.CriteriaSelect<RoleAlliance>(nHCriteriaalliance);
                    applylist.Add(ChangeDataType(alliance));

                }
                for (int i = 0; i < member.Count; i++)
                {
                    NHCriteria nHCriteriaalliance = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
                    var alliance = NHibernateQuerier.CriteriaSelect<RoleAlliance>(nHCriteriaalliance);
                    memberlist.Add(ChangeDataType(alliance));
                }

                Dictionary<byte, object> dict = new Dictionary<byte, object>();
                dict.Add((byte)ParameterCode.AllianceMember, memberlist);
                dict.Add((byte)ParameterCode.ApplyMember, applylist);
                RoleStatusSuccessS2C(roleID, AllianceOpCode.GetAllianceMember, dict);
            }
            else
                RoleStatusFailS2C(roleID,AllianceOpCode.GetAllianceMember);
        }

        #endregion


        AllianceMember ChangeDataType(AllianceMemberDTO memberDTO)
        {
            AllianceMember alliance = new AllianceMember();
            alliance.AllianceID = memberDTO.AllianceID;
            alliance.ApplyforMember =Utility.Json.ToJson(memberDTO.ApplyforMember) ;
            alliance.Member = Utility.Json.ToJson(memberDTO.Member);
            return alliance;
        }

        AllianceMemberDTO ChangeDataType(AllianceMember member)
        {
            AllianceMemberDTO allianceDTO = new AllianceMemberDTO();
            allianceDTO.AllianceID = member.AllianceID;
            allianceDTO.ApplyforMember = Utility.Json.ToObject<List<int>>(member.ApplyforMember);
            allianceDTO.Member = Utility.Json.ToObject<List<int>>(member.Member);
            return allianceDTO;
        }

        AllianceStatus ChangeDataType(AllianceStatusDTO statusDTO)
        {
            AllianceStatus allianceStatus = new AllianceStatus();
            allianceStatus.AllianceLevel = statusDTO.AllianceLevel;
            allianceStatus.AllianceMaster = statusDTO.AllianceMaster;
            allianceStatus.AllianceName = statusDTO.AllianceName;
            allianceStatus.AllianceNumberPeople = statusDTO.AllianceNumberPeople;
            allianceStatus.AlliancePeopleMax = statusDTO.AlliancePeopleMax;
            allianceStatus.ID = statusDTO.ID;
            allianceStatus.Manifesto = statusDTO.Manifesto;
            allianceStatus.OnLineNum = statusDTO.OnLineNum;
            allianceStatus.Popularity = statusDTO.Popularity;
            return allianceStatus;
        }

        
    }
}
