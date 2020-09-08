using Cosmos;
namespace AscensionServer
{
    /// <summary>
    /// 网络事件；
    /// key为short类型约束的枚举；
    /// value为object类型的对象
    /// </summary>
    public class OpCodeEventCore:ConcurrentEventCore<ushort,object,OpCodeEventCore>
    {
        //发送网络事件
    }
}
