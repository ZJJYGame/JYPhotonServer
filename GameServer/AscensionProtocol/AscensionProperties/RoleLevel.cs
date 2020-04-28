/*
*Author   Don
*Since 	2020-04-27
*Description 角色修炼境界枚举
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol
{
    public enum RoleLevel:byte
    {
        Lianqi_Level_1,//练气一层
        Lianqi_Level_2,//练气二层
        Lianqi_Level_3,//练气三层
        Lianqi_Level_4,//练气四层
        Lianqi_Level_5,//练气五层
        Lianqi_Level_6,//练气六层
        Lianqi_Level_7,//练气七层
        Lianqi_Level_8,//练气八层
        Lianqi_Level_9,//练气九层
        Lianqi_Level_Peakedness,//练气圆满
        Zhuji_Early,//筑基前期
        Zhuji_Medium,//筑基中期
        Zhuji_Later,//筑基后期
        Zhuji_Peakedness,//筑基巅峰
        Jiedan_Early,//结丹前期
        JIedan_Medium,//结丹中期
        JIedan_Later,//结丹后期
        JIedan_Peakedness,//结丹巅峰
        Yuanying_Early,//元婴前期
        Yuanying_Medium,//元婴中期
        Yuanying_Later,//元婴后期
        Yuanying_Peakedness,//元婴巅峰
        Huashen_Early,//化神前期
        Huashen_Medium,//化神中期
        Huashen_Later,//化神后期
        Huashen_Peakedness,//化神巅峰
    }
}
