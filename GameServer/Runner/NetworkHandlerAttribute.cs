using System;

    /// <summary>
    /// 网络消息处理特性，不可继承；
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class NetworkHandlerAttribute:Attribute
    {
    }
