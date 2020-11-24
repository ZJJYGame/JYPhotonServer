using System;
using System.Collections.Generic;
using System.Text;
namespace Cosmos
{
    public class Component : Object
    {
        public bool IsDisposed
        {
            get
            {
                return this.InstanceId == 0;
            }
        }
        private bool isFromPool;

        public bool IsFromPool
        {
            get
            {
                return this.isFromPool;
            }
            set
            {
                this.isFromPool = value;

                if (!this.isFromPool)
                {
                    return;
                }

                //if (this.InstanceId == 0)
                //{
                //    this.InstanceId = IdGenerater.GenerateInstanceId();
                //}
            }
        }
        public long InstanceId { get; private set; }
        Component parent;
        public Component Parent { get { return parent; } set { parent = value; } }
        public T GetParent<T>() where T : Component
        {
            return this.Parent as T;
        }
        public override void OnTermination()
        {
            if (IsDisposed)
                return;
            this.InstanceId = 0;
        }
    }
}