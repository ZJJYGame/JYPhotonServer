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
namespace AscensionServer
{
    public partial class GangsMananger
    {
        /// <summary>
        /// 申请加入宗门
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="id"></param>
        void ApplyJoinAllianceS2C(int roleID,int id)
        {
            var result = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAlliancePerfix, roleID.ToString()).Result;
            var alianceExits = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlliancePerfix, id.ToString()).Result;
            if (result&& alianceExits)
            {
                var roleAlliance= RedisHelper.Hash.HashGetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, roleID.ToString()).Result;
                var alliance = RedisHelper.Hash.HashGetAsync<AllianceMemberDTO>(RedisKeyDefine._AlliancePerfix, id.ToString()).Result;
                if (roleAlliance != null && alliance != null )
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

        async void  ApplyJoinAllianceMySql(int roleID, int id)
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
                    RoleStatusSuccessS2C(roleID, AllianceOpCode.JoinAlliance, ChangeDataType (roleAlliance));
                    //TODO更新到数据库
                    await NHibernateQuerier.UpdateAsync(roleAlliance);
                    await NHibernateQuerier.UpdateAsync(alliance);

                   await RedisHelper.Hash.HashSetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix,roleID.ToString(), ChangeDataType(roleAlliance));

                    await  RedisHelper.Hash.HashSetAsync<AllianceMemberDTO>(RedisKeyDefine._AllianceMemberPerfix, roleID.ToString(), ChangeDataType (alliance));

                }
                else
                {
                    RoleStatusFailS2C(roleID, AllianceOpCode.JoinAlliance);
                }
            }
            else
                RoleStatusFailS2C(roleID, AllianceOpCode.JoinAlliance);
        }


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
    }
}
