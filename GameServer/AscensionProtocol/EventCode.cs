﻿namespace AscensionProtocol
{
    public enum EventCode:byte//区分服务器像客户端发送事件的类型
    {
        NewPlayer=0,
        SyncPosition=1,
        DeletePlayer=2,
    }
}
