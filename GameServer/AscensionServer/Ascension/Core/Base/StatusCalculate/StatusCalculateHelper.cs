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
    public class StatusCalculateHelper
    {
        static int RoleHP { get; set; }
        static int RoleMP { get; set; }
        static int RoleSoul { get; set; }
        static int BestBlood { get; set; }
        static int AttackSpeed { get; set; }
        static int AttackPhysical { get; set; }
        static int AttackPower { get; set; }
        static int DefendPhysical { get; set; }
        static int DefendPower { get; set; }
        static int PhysicalCritProb { get; set; }
        static int MagicCritProb { get; set; }
        static int ReduceCritProb { get; set; }
        static int PhysicalCritDamage { get; set; }
        static int MagicCritDamage { get; set; }
        static int ReduceCritDamage { get; set; }
        static int MoveSpeed { get; set; }
        static int RolePopularity { get; set; }
        static int RoleMaxPopularity { get; set; }
        static int ValueHide { get; set; }
        public static void GetRoleStatus(List<GongFa> gongfaList, List<MishuSkillData> mishuList ,RoleStatusDatas roleStatusDatas, out RoleStatus roleStatus)
        {
            roleStatus = new RoleStatus();
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

                }
            }
            if (mishuList.Count > 0)
            {
                for (int i = 0; i < mishuList.Count; i++)
                {
                    AttackPhysical += mishuList[i].Attact_Physical;
                    AttackPower += mishuList[i].Attact_Power;
                    AttackSpeed += mishuList[i].Attact_Speed;
                    BestBlood += (short)mishuList[i].Best_Blood;
                    DefendPhysical += mishuList[i].Defend_Physical;
                    DefendPower += mishuList[i].Defend_Power;
                    //roleStatus.MagicCritDamage += gongfaList[i].;
                    //roleStatus.MagicCritProb += gongfaList[i].Attact_Physical;
                    MoveSpeed += mishuList[i].Move_Speed;
                    //roleStatus.PhysicalCritDamage += gongfaList[i].;
                    //roleStatus.PhysicalCritProb += gongfaList[i].Attact_Physical;
                    //roleStatus.ReduceCritDamage += gongfaList[i].Attact_Physical;
                    //roleStatus.ReduceCritProb += gongfaList[i].Attact_Physical;
                    RoleHP += mishuList[i].Role_HP;
                    RoleMP += mishuList[i].Role_MP;
                    RolePopularity += mishuList[i].Role_Popularity;
                    roleStatus.RoleSoul += mishuList[i].Role_Soul;
                    roleStatus.ValueHide += mishuList[i].Value_Hide;
                }
            }
            OutRolestatus(roleStatus, roleStatusDatas,out var tempstatus);
            roleStatus = tempstatus;
            Utility.Debug.LogInfo("yzqData角色属性加成为" + tempstatus.RoleHP);
        }

        public static void OutRolestatus(RoleStatus roleStatus, RoleStatusDatas roleStatusDatas,out RoleStatus tempstatus)
        {
            tempstatus = new RoleStatus();
            tempstatus.AttackPhysical= (int)((roleStatusDatas.AttackPhysical * (roleStatus.AttackPhysical/10000f))+ AttackPhysical);
            tempstatus.AttackPower = (int)((roleStatusDatas.AttackPower * (roleStatus.AttackPower / 10000f)) + AttackPower);
            tempstatus.AttackSpeed = (int)((roleStatusDatas.AttackSpeed *(roleStatus.AttackSpeed / 10000f)) + AttackSpeed);
            tempstatus.BestBlood = (short)((roleStatusDatas.BestBlood * (roleStatus.BestBlood / 10000f)) + BestBlood);
            tempstatus.DefendPhysical = (int)((roleStatusDatas.DefendPhysical *(roleStatus.DefendPhysical / 10000f)) + DefendPhysical);
            tempstatus.DefendPower = (int)((roleStatusDatas.DefendPower * (roleStatus.DefendPower / 10000f)) + DefendPower);
            tempstatus.MagicCritDamage = 0;
            tempstatus.MagicCritProb =0;
            tempstatus.MoveSpeed = (int)((roleStatusDatas.MoveSpeed * (roleStatus.MoveSpeed / 10000f)) + MoveSpeed);
            tempstatus.PhysicalCritDamage =0;
            tempstatus.PhysicalCritProb = 0;
            tempstatus.ReduceCritDamage =0;
            tempstatus.ReduceCritProb =0;
            tempstatus.RoleHP =(int)((roleStatusDatas.RoleHP * (roleStatus.RoleHP / 10000f)) + RoleHP);
            tempstatus.RoleMP = (int)((roleStatusDatas.RoleMP * (roleStatus.RoleMP / 10000f)) + RoleMP);
            tempstatus.RolePopularity = (int)((roleStatusDatas.RolePopularity * (roleStatus.RolePopularity / 10000f)) + RolePopularity);
            tempstatus.RoleSoul = (int)((roleStatusDatas.RoleSoul * (roleStatus.RoleSoul / 10000f)) + RoleSoul);
            tempstatus.ValueHide = (int)((roleStatusDatas.ValueHide *(roleStatus.ValueHide / 10000f)) + ValueHide);

        }

        public static void UpdateRoleStatus(RoleStatus roleStatusold, RoleStatus roleStatusnew,out RoleStatus roleStatus)
        {
            Utility.Debug.LogInfo("yzqData角色属性加成为" + Utility.Json.ToJson(roleStatusold)+"新的属性/n"+ Utility.Json.ToJson(roleStatusnew));
            roleStatus = new RoleStatus();
            if (roleStatusold.RoleMaxHP< roleStatusnew.RoleHP)
            {
                roleStatusold.RoleHP += (roleStatusnew.RoleHP - roleStatusold.RoleMaxHP);
                roleStatusold.RoleMaxHP = roleStatusnew.RoleHP;
            }

            if (roleStatusold.RoleMaxMP < roleStatusnew.RoleMP)
            {
                roleStatusold.RoleMP += (roleStatusnew.RoleMP - roleStatusold.RoleMaxMP);
                roleStatusold.RoleMaxMP = roleStatusnew.RoleMP;
            }

            if (roleStatusold.BestBloodMax < roleStatusnew.BestBlood)
            {
                roleStatusold.BestBlood +=(short)(roleStatusnew.BestBlood - roleStatusold.BestBloodMax);
                roleStatusold.BestBloodMax = roleStatusnew.BestBlood;
            }

            if (roleStatusold.RoleMaxPopularity < roleStatusnew.RolePopularity)
            {
                roleStatusold.RolePopularity += (short)(roleStatusnew.RolePopularity - roleStatusold.RoleMaxPopularity);
                roleStatusold.RoleMaxPopularity = roleStatusnew.RolePopularity;
            }

            if (roleStatusold.RoleMaxSoul < roleStatusnew.RoleSoul)
            {
                roleStatusold.RoleSoul += (short)(roleStatusnew.RoleSoul - roleStatusold.RoleMaxSoul);
                roleStatusold.RoleMaxSoul = roleStatusnew.RoleSoul;
            }
            roleStatus = roleStatusold;
        }
    }
}
