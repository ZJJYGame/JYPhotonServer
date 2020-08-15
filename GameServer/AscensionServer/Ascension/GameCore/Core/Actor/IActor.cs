using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AscensionServer
{
    public interface IActor<T>
        where T:class
    {
        /// <summary>
        /// Actor类型;
        /// 0：玩家；
        /// 1：宠物；
        /// 2：AI类型01
        /// 3：AI类型02
        ///      etc . . . 
        /// 查看 ：ActorTypeEnum
        /// </summary>
        byte ConcreteActorType { get; }
        /// <summary>
        /// 系统生成的持久化ID
        /// 可以是Peer的 role id;
        /// 也可以是pet id;
        /// 亦可是80001这类AI怪物
        /// </summary>
        int ConcreteActorID { get; }
        /// <summary>
        /// 获得具体的Actor
        /// </summary>
        /// <returns>具体的Actor类型</returns>
        T GetConcreteActor();
        /// <summary>
        /// 设置具体actor
        /// </summary>
        /// <param name="concreteActor"></param>
        void SetConcreteActor(T concreteActor);
        /// <summary>
        /// 设置Acotor数据
        /// </summary>
        /// <typeparam name="TData">数据类型，无约束</typeparam>
        /// <param name="dataKey">数据类型</param>
        /// <param name="data">具体数据</param>
        void SetData<TData>(short dataKey, TData data)where TData: Variable;
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="TData">数据类型，无约束</typeparam>
        /// <param name="dataKey">数据类型</param>
        /// <returns>具体数据</returns>
        TData GetData<TData>(short dataKey) where TData : Variable;
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="dataKey">数据类型</param>
        /// <returns>是否移除成功</returns>
        bool RemoveData(short dataKey);
        /// <summary>
        /// 是否存在数据
        /// </summary>
        /// <param name="dataKey">数据类型</param>
        /// <returns>是否存在</returns>
        bool HasData(short dataKey);
    }
}
