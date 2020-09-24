﻿using AscensionProtocol;
using Cosmos;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [TargetHelper]
    public class PhotonMessageHelper : INetworkMessageHelper
    {
        Dictionary<byte, IHandler> handlerDict;
        public PhotonMessageHelper()
        {
            handlerDict = new Dictionary<byte, IHandler>();
            InitHandler();
        }
        public object EncodeMessage(object message)
        {
            if (message is OperationRequest)
                Utility.Debug.LogInfo("EncodeMessage 验证成功");
            OperationRequest request = message as OperationRequest;
            IHandler handler;
            var result = handlerDict.TryGetValue(request.OperationCode, out handler);
            if (!result)
                return handlerDict[0].EncodeMessage(request);//使用default handler
            else
                return handler.EncodeMessage(request);
        }
        void InitHandler()
        {
            var handlers = Utility.Assembly.GetInstancesByAttribute<InheritedAttribute, IHandler>(true);
            int length = handlers.Length;
            for (int i = 0; i < length; i++)
            {
                handlers[i].OnInitialization();
                handlerDict.Add(handlers[i].OpCode, handlers[i]);
            }
        }
    }
}
