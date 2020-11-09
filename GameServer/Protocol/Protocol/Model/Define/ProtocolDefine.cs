using System;

namespace Protocol
{
    /// <summary>
    /// 注意；协议数字范围为一个ushort的取值区间；
    /// </summary>
    public class ProtocolDefine
    {
        #region Port  Range[4096 , 4224] ; Count:128;
        /// <summary>
        /// 请求端口；
        /// </summary>
        public const ushort PORT_REQUEST = 4319;
        /// <summary>
        /// 输入同步端口；
        /// </summary>
        public const ushort PORT_INPUT = 4320;
        /// <summary>
        /// 聊天端口
        /// </summary>
        public const ushort PORT_CHAT = 4321;
        #endregion

        #region Operation Range[0 , 255] ; Count:256; OPR=Operation
        /// <summary>
        /// 测试通道；
        /// </summary>
        public const ushort OPR_TESTCAHNNEL = 336;
        /// <summary>
        /// PlayerInput;
        /// 玩家在高同步场景中的输入；
        /// 例如探索界面，副本界面等；
        /// </summary>
        public const ushort OPR_PLYAER_INPUT = 5;
        /// <summary>
        /// 玩家的角色登录；注意区分账号登录；
        /// </summary>
        public const ushort OPR_PLYAER_LOGIN = 6;
        /// <summary>
        /// 玩家的角色登出；注意区分账号登录；
        /// </summary>
        public const ushort OPR_PLYAER_LOGOFF = 7;
        /// <summary>
        /// PlayerEnter；
        /// 玩家进入高同步场景，例如探索界面，副本界面等；
        /// </summary>
        public const ushort OPR_PLAYER_ENTER = 111;
        /// <summary>
        /// PlayerExit
        /// 玩家离开高同步场景，例如探索界面，副本界面等；
        /// </summary>
        public const ushort OPR_PLAYER_EXIT = 112;
        /// <summary>
        /// 高同步场景中的技能使用。例如探索界面，副本界面等；
        /// </summary>
        public const ushort OPR_PLAYER_SKILL = 36;
        #endregion

        #region ReturnCode  Range[256 , 319] ; Count:64; RTN=Return
        /// <summary>
        /// 成功；
        /// </summary>
        public const short RTN_SUCCESS = 257;
        /// <summary>
        /// 错误
        /// </summary>
        public const short RTN_ERROR = 258;
        /// <summary>
        /// 失败；
        /// </summary>
        public const short RTN_FAIL = 259;
        /// <summary>
        /// 空；
        /// </summary>
        public const short RTN_EMPTY = 260;
        /// <summary>
        /// 操作码无效；
        /// </summary>
        public const short RTN_INVALIDOPERATION = 261;
        /// <summary>
        /// 数据无效；
        /// </summary>
        public const short RTN_INVALIDDATA = 262;
        /// <summary>
        /// 条款（Item）已存在；
        /// </summary>
        public const short RTN_ALREADYEXISTS = 263;
        /// <summary>
        /// 未查询到条款（Item）；
        /// </summary>
        public const short RTN_NOTFOUND = 264;
        #endregion

        #region CMD Range[320 , 335] ; Count:16; CMD=Command
        /// <summary>
        /// CMD指令;
        /// 若无必要，勿动；
        /// </summary>
        public const ushort CMD_MSG = 320;
        public const ushort CMD_SYN = 321;
        public const ushort CMD_ACK = 322;
        public const ushort CMD_FIN = 323;
        #endregion
    }
}
