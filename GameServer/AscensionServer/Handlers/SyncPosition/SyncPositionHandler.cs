﻿/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 同步位置
*/
using AscensionProtocol;
using Photon.SocketServer;
namespace AscensionServer
{
  public  class SyncPositionHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncPositon;
            base.OnInitialization();
        }
        //获取客户端位置请求的处理的代码
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            //接收位置并保持起来
            float x = (float)Utility.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.x);
            float y = (float)Utility.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.y);
            float z = (float)Utility.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.z);

            peer.x = x; peer.y = y; peer.z = z;//把位置数据传递给Clientpeer保存管理起来

            AscensionServer._Log.Info(x + "--" + y + "--" + z);//输出测试
        }
       
    }
}
