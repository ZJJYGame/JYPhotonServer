/*
*Author : Don
*Since 	:2020-05-06
*Description  : 常用错误码
*/
namespace AscensionProtocol
{
    public enum ErrorCode:short
    {
        InvalidRequestParameters = -6,//无效请求码
        ArgumentOutOfRange = -4,//参数越界

        OperationDenied = -3,//拒绝操作
        OperationInvalid = -2,//无效操作
        InternalServerError = -1,//服务器内部错误

        Ok = 0,

        InvalidAuthentication = 32767, // 0x7FFF, // codes start at short.MaxValue 无效权限
        GameIdAlreadyExists = 32766, // 0x7FFF - 1,游戏ID已存在
        GameFull = 32765, // 0x7FFF - 2, 达到游戏容量上限
        GameClosed = 32764, // 0x7FFF - 3,游戏关闭
        AlreadyMatched = 32763, // 0x7FFF - 4,已匹配
        ServerFull = 32762, // 0x7FFF - 5,服务武器满
        UserBlocked = 32761, // 0x7FFF - 6,用户屏蔽
        NoMatchFound = 32760, // 0x7FFF - 7,匹配未找到
        RedirectRepeat = 32759, // 0x7FFF - 8,定向重复
        GameIdNotExists = 32758, // 0x7FFF - 9,游戏ID不存在
    }
}
