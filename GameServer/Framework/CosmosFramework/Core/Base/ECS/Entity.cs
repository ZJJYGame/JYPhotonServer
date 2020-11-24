using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos
{
	public class Entity : ComponentWithId
	{
		protected Entity(int id) : base(id) { }
		protected Entity() { }
		private Dictionary<Type, Component> componentDict = new Dictionary<Type, Component>();
		private HashSet<Component> components = new HashSet<Component>();
		public override void OnTermination()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.OnTermination();

			foreach (Component component in this.componentDict.Values)
			{
				try
				{
					component.OnTermination();
				}
				catch (Exception e)
				{
					Utility.Debug.LogError(e);
				}
			}

			this.components.Clear();
			this.componentDict.Clear();
		}
		/// <summary>
		///此方法当前不可用 ！
		/// </summary>
		public virtual Component AddComponent(Component component)
		{
			Type type = component.GetType();
			if (this.componentDict.ContainsKey(type))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {type.Name}");
			}

			component.Parent = this;

			this.componentDict.Add(type, component);

			if (component is ISerializeToEntity)
			{
				this.components.Add(component);
			}

			return component;
		}
		/// <summary>
		///此方法当前不可用 ！
		/// </summary>
		public virtual Component AddComponent(Type type)
		{
			if (this.componentDict.ContainsKey(type))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {type.Name}");
			}

			//Component component = ComponentFactory.CreateWithParent(type, this, this.IsFromPool);
			Component component = default;

			this.componentDict.Add(type, component);

			if (component is ISerializeToEntity)
			{
				this.components.Add(component);
			}

			return component;
		}
		/// <summary>
		///此方法当前不可用 ！
		/// </summary>
		public virtual K AddComponent<K>() where K : Component, new()
		{
			Type type = typeof(K);
			if (this.componentDict.ContainsKey(type))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
			}

			//K component = ComponentFactory.CreateWithParent<K>(this, this.IsFromPool);
			K component = default;

			this.componentDict.Add(type, component);

			if (component is ISerializeToEntity)
			{
				this.components.Add(component);
			}

			return component;
		}
		/// <summary>
		///此方法当前不可用 ！
		/// </summary>
		public virtual K AddComponent<K, P1>(P1 p1) where K : Component, new()
		{
			Type type = typeof(K);
			if (this.componentDict.ContainsKey(type))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
			}

			//K component = ComponentFactory.CreateWithParent<K, P1>(this, p1, this.IsFromPool);
			K component = default;

			this.componentDict.Add(type, component);

			if (component is ISerializeToEntity)
			{
				this.components.Add(component);
			}

			return component;
		}
		/// <summary>
		///此方法当前不可用 ！
		/// </summary>
		public virtual K AddComponent<K, P1, P2>(P1 p1, P2 p2) where K : Component, new()
		{
			Type type = typeof(K);
			if (this.componentDict.ContainsKey(type))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
			}

			//K component = ComponentFactory.CreateWithParent<K, P1, P2>(this, p1, p2, this.IsFromPool);
			K component = default;

			this.componentDict.Add(type, component);

			if (component is ISerializeToEntity)
			{
				this.components.Add(component);
			}

			return component;
		}
		/// <summary>
		///此方法当前不可用 ！
		/// </summary>
		public virtual K AddComponent<K, P1, P2, P3>(P1 p1, P2 p2, P3 p3) where K : Component, new()
		{
			Type type = typeof(K);
			if (this.componentDict.ContainsKey(type))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
			}

			//K component = ComponentFactory.CreateWithParent<K, P1, P2, P3>(this, p1, p2, p3, this.IsFromPool);
			K component = default;


			this.componentDict.Add(type, component);

			if (component is ISerializeToEntity)
			{
				this.components.Add(component);
			}

			return component;
		}
		/// <summary>
		///此方法当前不可用 ！
		/// </summary>
		public virtual void RemoveComponent<K>() where K : Component
		{
			if (this.IsDisposed)
			{
				return;
			}
			Type type = typeof(K);
			Component component;
			if (!this.componentDict.TryGetValue(type, out component))
			{
				return;
			}

			this.componentDict.Remove(type);
			this.components.Remove(component);

			component.OnTermination();
		}
		/// <summary>
		///此方法当前不可用 ！
		/// </summary>
		public virtual void RemoveComponent(Type type)
		{
			if (this.IsDisposed)
			{
				return;
			}
			Component component;
			if (!this.componentDict.TryGetValue(type, out component))
			{
				return;
			}

			this.componentDict.Remove(type);
			this.components.Remove(component);

			component.OnTermination();
		}
		/// <summary>
		///此方法当前不可用 ！
		/// </summary>
		public K GetComponent<K>() where K : Component
		{
			Component component;
			if (!this.componentDict.TryGetValue(typeof(K), out component))
			{
				return default(K);
			}
			return (K)component;
		}
		/// <summary>
		///此方法当前不可用 ！
		/// </summary>
		public Component GetComponent(Type type)
		{
			Component component;
			if (!this.componentDict.TryGetValue(type, out component))
			{
				return null;
			}
			return component;
		}
		/// <summary>
		///此方法当前不可用 ！
		/// </summary>
		public Component[] GetComponents()
		{
			return this.componentDict.Values.ToArray();
		}
	}
	//Dictionary<GenericValuePair<Type, byte>, Component> compDict = new Dictionary<GenericValuePair<Type, byte>, Component>();
}
