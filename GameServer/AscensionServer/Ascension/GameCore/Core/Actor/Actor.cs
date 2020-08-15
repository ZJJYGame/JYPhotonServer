using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    /// <summary>
    /// 此类是一个容器；
    /// 能够缓存T类型的对象数据，并为T类型对象追加附加数据；
    /// 当为T类型对象追加数据时，无需打开T类进行代码重构；
    /// 例如为Peer对象追加数据，则无需修改peer，只需添加Variable即可
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Actor<T> : ActorBase,IActor<T>
        where T : class
    {
        public override Type ActorType { get { return (typeof(T)); } }
        /// <summary>
        /// 当前Actor是否处于激活状态
        /// </summary>
        public override bool IsActive { get { return concreteActor == null; } }
        public override byte ConcreteActorType { get; set; }
        public override int ConcreteActorID { get ; set ; }
        T concreteActor;
        Dictionary<short, Variable> actorDataDict = new Dictionary<short, Variable>();
        public T GetConcreteActor()
        {
            return concreteActor;
        }
        public TData GetData<TData>(short dataKey)
            where TData : Variable
        {
            if (actorDataDict.ContainsKey(dataKey))
                return actorDataDict[dataKey] as TData;
            else
                return default;
        }
        public bool HasData(short dataKey)
        {
            return actorDataDict.ContainsKey(dataKey);
        }
        public bool RemoveData(short dataKey)
        {
            if (actorDataDict.ContainsKey(dataKey))
                return actorDataDict.Remove(dataKey);
            else
                return false;
        }
        public void SetConcreteActor(T concreteActor)
        {
            this.concreteActor = concreteActor;
        }
        public void SetData<TData>(short dataKey, TData data)
            where TData : Variable
        {
            if (!actorDataDict.ContainsKey(dataKey))
                actorDataDict.Add(dataKey,data);
        }
    }
}
