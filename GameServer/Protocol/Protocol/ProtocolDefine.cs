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

        #region ReturnCode  Range[256 , 319] ; Count:64;
        /// <summary>
        /// 成功；
        /// </summary>
        public const short RETURN_SUCCESS = 257;
        /// <summary>
        /// 错误
        /// </summary>
        public const short RETURN_ERROR = 258;
        /// <summary>
        /// 失败；
        /// </summary>
        public const short RETURN_FAIL = 259;
        /// <summary>
        /// 空；
        /// </summary>
        public const short RETURN_EMPTY = 260;
        /// <summary>
        /// 操作码无效；
        /// </summary>
        public const short RETURN_INVALIDOPERATION = 261;
        /// <summary>
        /// 数据无效；
        /// </summary>
        public const short RETURN_INVALIDDATA = 262;
        /// <summary>
        /// 条款（Item）已存在；
        /// </summary>
        public const short RETURN_ALREADYEXISTS = 263;
        /// <summary>
        /// 未查询到条款（Item）；
        /// </summary>
        public const short RETURN_NOTFOUND = 264;
        #endregion

        #region CMD Range[320 , 335] ; Count:16;
        /// <summary>
        /// CMD指令;
        /// 若无必要，勿动；
        /// </summary>
        public const ushort CMD_MSG = 320;
        public const ushort CMD_SYN = 321;
        public const ushort CMD_ACK = 322;
        public const ushort CMD_FIN = 323;
        #endregion

        #region Operation Range[336 , 367] ; Count:32;
        /// <summary>
        /// 测试通道；
        /// </summary>
        public const ushort OPERATION_TESTCAHNNEL = 336;
        /// <summary>
        /// PlayerInput;
        /// </summary>
        public const ushort OPERATION_PLYAERINPUT = 337;
        /// <summary>
        /// EnterRoom;
        /// </summary>
        public const ushort OPERATION_ENTERROOM = 338;
        /// <summary>
        /// ExitRoom;
        /// </summary>
        public const ushort OPERATION_EXITROOM = 339;
        /// <summary>
        /// PlayerEnter
        /// </summary>
        public const ushort OPERATION_PLAYERENTER = 340;
        /// <summary>
        /// PlayerExit
        /// </summary>
        public const  ushort OPERATION_PLAYEREXIT = 341;
        #endregion
    }
}
