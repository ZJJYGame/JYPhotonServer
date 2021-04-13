﻿using AscensionProtocol.DTO;
using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class SkillDataVerifyHelper : IDataVerifyHelper
    {
        RoleStatusDTO rsd;
   
        bool VerifyCost(FixAffectValue value, int playerId)
        {
            //计算规则：
            //最终消耗数值= 最大数值*消耗百分比+消耗固定数值
            bool valid = false;
            switch (value.AffectType)
            {
                case SkillDefine.Affectable_Speed:
                    break;
                case SkillDefine.Affectable_Qixue:
                    {
                        var costVar = rsd.RoleMaxMP * value.AffectPercent + value.AffectValue;
                        if (rsd.RoleMaxHP >= costVar)
                        {
                            valid = true;
                        }
                    }
                    break;
                case SkillDefine.Affectable_Zhenyuan:
                    {
                        var costVar = rsd.RoleMaxMP * value.AffectPercent + value.AffectValue;
                        if (rsd.RoleMP >= costVar)
                        {
                            valid = true;
                        }
                    }
                    break;
                case SkillDefine.Affectable_Shenhun:
                    {
                        var costVar = rsd.RoleMaxMP * value.AffectPercent + value.AffectValue;
                        if (rsd.RoleMaxSoul >= costVar)
                        {
                            valid = true;
                        }
                    }
                    break;
                case SkillDefine.Affectable_Cloak:
                    break;
                case SkillDefine.Affectable_Jingxue:
                    {
                        var costVar = rsd.RoleMaxMP * value.AffectPercent + value.AffectValue;
                        if (rsd.BestBloodMax >= costVar)
                        {
                            valid = true;
                        }
                    }
                    break;
                case SkillDefine.Affectable_Transform:
                    break;
            }
            return valid;
        }
        bool VerifyBouns(FixAffectValue value)
        {
            //计算规则：
            //最终增益数值= 最大数值*消耗百分比+增益固定数值
            bool valid = false;
            switch (value.AffectType)
            {
                case SkillDefine.Affectable_Speed:
                    break;
                case SkillDefine.Affectable_Qixue:
                    break;
                case SkillDefine.Affectable_Zhenyuan:
                    {
                        var bounsVar = rsd.RoleMaxMP * value.AffectPercent + value.AffectValue;
                        var bounsedVar = rsd.RoleMP + bounsVar;
                        if (bounsedVar>=rsd.RoleMaxMP)
                        {
                            bounsedVar = rsd.RoleMaxMP;
                            valid = true;
                        }
                    }
                    break;
                case SkillDefine.Affectable_Shenhun:
                    break;
                case SkillDefine.Affectable_Cloak:
                    break;
                case SkillDefine.Affectable_Jingxue:
                    break;
                case SkillDefine.Affectable_Transform:
                    break;
            }
            return valid;
        }
    }
}


