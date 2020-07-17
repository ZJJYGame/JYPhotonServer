﻿namespace AscensionProtocol
{
    public enum EventCode:byte//区分服务器像客户端发送事件的类型
    {
        NewPlayer=0,
        DeletePlayer=1,
        ReplacePlayer=2,
        SyncRoleTransform=3,
        SyncRoleMoveStatus = 4,
        /// <summary>
        /// 占用资源单位事件
        /// </summary>
        OccupiedResourceUnit = 5,
        SyncMonsterTransform = 6,
        RelieveOccupiedResourceUnit=7,
    }
}
