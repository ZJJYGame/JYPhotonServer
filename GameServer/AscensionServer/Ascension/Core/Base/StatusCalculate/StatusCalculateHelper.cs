using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using Cosmos;

namespace AscensionServer
{
    //[Obsolete("弃用，当前线程不安全",true)]
    public class StatusCalculateHelper
    {
        public static void GetRoleStatus(List<GongFa> gongfaList, List<MishuSkillData> mishuList ,RoleStatusDatas roleStatusDatas, out RoleStatus roleStatus)
        {
          var roleStatustemp=  GameManager.ReferencePoolManager.Spawn<RoleStatus>();
            roleStatustemp.Clear();
            roleStatus = GameManager.ReferencePoolManager.Spawn<RoleStatus>();
            if (gongfaList.Count > 0)
            {
                for (int i = 0; i < gongfaList.Count; i++)
                {
                    roleStatus.AttackPhysical += gongfaList[i].Attact_Physical;
                    roleStatus.AttackPower += gongfaList[i].Attact_Power;
                    roleStatus.AttackSpeed += gongfaList[i].Attact_Speed;
                    roleStatus.BestBlood += (short)gongfaList[i].Best_Blood;
                    roleStatus.DefendPhysical += gongfaList[i].Defend_Physical;
                    roleStatus.DefendPower += gongfaList[i].Defend_Power;
                    //roleStatus.MagicCritDamage += gongfaList[i].;
                    //roleStatus.MagicCritProb += gongfaList[i].Attact_Physical;
                    roleStatus.MoveSpeed += gongfaList[i].Move_Speed;
                    //roleStatus.PhysicalCritDamage += gongfaList[i].;
                    //roleStatus.PhysicalCritProb += gongfaList[i].Attact_Physical;
                    //roleStatus.ReduceCritDamage += gongfaList[i].Attact_Physical;
                    //roleStatus.ReduceCritProb += gongfaList[i].Attact_Physical;
                    roleStatus.RoleHP += gongfaList[i].Role_HP;
                    roleStatus.RoleMP += gongfaList[i].Role_MP;
                    roleStatus.RolePopularity += gongfaList[i].Role_Popularity;
                    roleStatus.RoleSoul += gongfaList[i].Role_Soul;
                    roleStatus.ValueHide += gongfaList[i].Value_Hide;
                    roleStatus.GongfaLearnSpeed += gongfaList[i].Gongfa_LearnSpeed;
                    roleStatus.MishuLearnSpeed += gongfaList[i].Mishu_LearnSpeed;
                }
            }
            if (mishuList.Count > 0)
            {
                for (int i = 0; i < mishuList.Count; i++)
                {
                    roleStatustemp.AttackPhysical += mishuList[i].Attact_Physical;
                    roleStatustemp.AttackPower += mishuList[i].Attact_Power;
                    roleStatustemp.AttackSpeed += mishuList[i].Attact_Speed;
                    roleStatustemp.BestBlood += (short)mishuList[i].Best_Blood;
                    roleStatustemp.DefendPhysical += mishuList[i].Defend_Physical;
                    roleStatustemp.DefendPower += mishuList[i].Defend_Power;
                    //roleStatus.MagicCritDamage += gongfaList[i].;
                    //roleStatus.MagicCritProb += gongfaList[i].Attact_Physical;
                    roleStatustemp.MoveSpeed += mishuList[i].Move_Speed;
                    //roleStatus.PhysicalCritDamage += gongfaList[i].;
                    //roleStatus.PhysicalCritProb += gongfaList[i].Attact_Physical;
                    //roleStatus.ReduceCritDamage += gongfaList[i].Attact_Physical;
                    //roleStatus.ReduceCritProb += gongfaList[i].Attact_Physical;
                    roleStatustemp.RoleHP += mishuList[i].Role_HP;
                    roleStatustemp.RoleMP += mishuList[i].Role_MP;
                    roleStatustemp.RolePopularity += mishuList[i].Role_Popularity;
                    roleStatustemp.RoleSoul += mishuList[i].Role_Soul;
                    roleStatustemp.ValueHide += mishuList[i].Value_Hide;
                    //roleStatus.MishuLearnSpeed += mishuList[i].Role_Soul;
                    //roleStatus.ValueHide += mishuList[i].Value_Hide;
                }
            }
            OutRolestatus(roleStatus, roleStatusDatas, roleStatustemp, out var tempstatus);
            roleStatus = tempstatus;
           // GameManager.ReferencePoolManager.Despawns(roleStatustemp, roleStatus);
        }

        public static void OutRolestatus(RoleStatus roleStatus, RoleStatusDatas roleStatusDatas, RoleStatus roleStatusTemp, out RoleStatus tempstatus)
        {
            tempstatus = new RoleStatus();
            tempstatus.AttackPhysical= (int)((roleStatusDatas.AttackPhysical * (roleStatus.AttackPhysical/10000f))+ roleStatusTemp.AttackPhysical);
            Utility.Debug.LogInfo("yzqData角色属性加成为" + roleStatusDatas.AttackPhysical + ">>>"+ roleStatus.AttackPhysical / 10000f+"》》》》"+ roleStatusTemp.AttackPhysical);
            tempstatus.AttackPower = (int)((roleStatusDatas.AttackPower * (roleStatus.AttackPower / 10000f)) + roleStatusTemp.AttackPower);
            tempstatus.AttackSpeed = (int)((roleStatusDatas.AttackSpeed *(roleStatus.AttackSpeed / 10000f)) + roleStatusTemp.AttackSpeed);
            tempstatus.BestBlood = (short)((roleStatusDatas.BestBlood * (roleStatus.BestBlood / 10000f)) + roleStatusTemp.BestBlood);
            tempstatus.BestBloodMax = tempstatus.BestBlood;
            tempstatus.DefendPhysical = (int)((roleStatusDatas.DefendPhysical *(roleStatus.DefendPhysical / 10000f)) + roleStatusTemp.DefendPhysical);
            tempstatus.DefendPower = (int)((roleStatusDatas.DefendPower * (roleStatus.DefendPower / 10000f)) + roleStatusTemp.DefendPower);
            tempstatus.MagicCritDamage = 0;
            tempstatus.MagicCritProb =0;
            tempstatus.MoveSpeed = (int)((roleStatusDatas.MoveSpeed * (roleStatus.MoveSpeed / 10000f)) + roleStatusTemp.MoveSpeed);
            tempstatus.PhysicalCritDamage =0;
            tempstatus.PhysicalCritProb = 0;
            tempstatus.ReduceCritDamage =0;
            tempstatus.ReduceCritProb =0;
            tempstatus.RoleHP =(int)((roleStatusDatas.RoleHP * (roleStatus.RoleHP / 10000f)) + roleStatusTemp.RoleHP);
            tempstatus.RoleMaxHP = tempstatus.RoleHP;
            tempstatus.RoleMP = (int)((roleStatusDatas.RoleMP * (roleStatus.RoleMP / 10000f)) + roleStatusTemp.RoleMP);
            tempstatus.RoleMaxMP = tempstatus.RoleMP;
            tempstatus.RolePopularity = (int)((roleStatusDatas.RolePopularity * (roleStatus.RolePopularity / 10000f)) + roleStatusTemp.RolePopularity);
            tempstatus.RoleMaxPopularity = tempstatus.RolePopularity;
            tempstatus.RoleSoul = (int)((roleStatusDatas.RoleSoul * (roleStatus.RoleSoul / 10000f)) + roleStatusTemp.RoleSoul);
            tempstatus.RoleMaxSoul = tempstatus.RoleSoul;
            tempstatus.ValueHide = (int)((roleStatusDatas.ValueHide *(roleStatus.ValueHide / 10000f)) + roleStatusTemp.ValueHide);
            tempstatus.MishuLearnSpeed = (int)((roleStatusDatas.MishuLearnSpeed * (roleStatus.MishuLearnSpeed / 10000f)) + roleStatusTemp.MishuLearnSpeed);
            tempstatus.GongfaLearnSpeed = (int)((roleStatusDatas.GongfaLearnSpeed * (roleStatus.GongfaLearnSpeed / 10000f)) + roleStatusTemp.GongfaLearnSpeed);

        }

    }
}
