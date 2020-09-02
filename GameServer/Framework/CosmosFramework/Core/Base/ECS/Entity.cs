using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos
{
    public class Entity : ComponentWithId
    {
        Dictionary<Type, Component> compDict = new Dictionary<Type, Component>();
        public virtual Component AddComponent(Component component)
        {
            Type type = component.GetType();
            if (this.compDict.ContainsKey(type))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {type.Name}");
            }
            component.Parent = this;
            return component;

        }
        public virtual Component AddComponent(Type type)
        {
            if (this.compDict.ContainsKey(type))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {type.Name}");
            }
            Component component =Utility.Assembly.GetTypeInstance<Component>(type);
            component.Parent = this;
            this.compDict.Add(type, component);
            return component;
        }
    }
}
