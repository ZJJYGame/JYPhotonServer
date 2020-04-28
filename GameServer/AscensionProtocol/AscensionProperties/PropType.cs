/*
*Author   Don
*Since 	2020-04-27
*Description 道具类型
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol { 
    public  enum PropType:byte
    {
        Material,//材料
        Damage,//伤害
        healing,//回复
        Shield,//护盾
        Buff//增益状态
    }
}
