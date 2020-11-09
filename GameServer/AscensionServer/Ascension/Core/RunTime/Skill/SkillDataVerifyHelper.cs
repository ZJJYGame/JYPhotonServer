using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class SkillDataVerifyHelper : IDataVerifyHelper
    {
        public bool VerifyData(IDataContract data)
        {
            var skill = data as C2SSkillInput;
            var costs= skill.Costs;
            if (costs != null)
            {
                for (int i = 0; i < costs.Count; i++)
                {
                    if (!VerifyCost(costs[i], skill.PlayerId))
                        return false;
                }
            }
            var bouns = skill.Bouns;
            if (bouns != null)
            {
                for (int i = 0; i < bouns.Count; i++)
                {
                    if (!VerifyBouns(bouns[i], skill.PlayerId))
                        return false;
                }
            }
            return false;
        }
        bool VerifyCost(FixAffectValue value,int playerId)
        {
            bool valid = false;
            switch (value.AffectType)
            {
                case SkillDefine.Affectable_Speed:
                    break;
                case SkillDefine.Affectable_Qixue:
                    break;
                case SkillDefine.Affectable_Zhenyuan:
                    break;
                case SkillDefine.Affectable_Shenhun:
                    {

                    }
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
        bool VerifyBouns(FixAffectValue value, int playerId)
        {
            bool valid = false;
            switch (value.AffectType)
            {
                case SkillDefine.Affectable_Speed:
                    break;
                case SkillDefine.Affectable_Qixue:
                    break;
                case SkillDefine.Affectable_Zhenyuan:

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
