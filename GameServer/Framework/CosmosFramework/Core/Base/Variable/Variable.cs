using System;
namespace Cosmos
{
    public abstract class Variable :  IRenewable
    {
        protected Variable() { }
        /// <summary>
        /// 变量类型
        /// </summary>
        public abstract Type Type { get; }
        /// <summary>
        /// 空虚函数；
        /// 获取变量值
        /// </summary>
        /// <returns></returns>
        public abstract object GetValue();
        /// <summary>
        /// 空虚函数；
        /// 设置变量值
        /// </summary>
        /// <param name="value">变量值</param>
        public abstract void SetValue(object value);
        /// <summary>
        /// 空虚函数；
        /// 重置变量;
        /// </summary>
        public virtual void OnRenewal() { }
    }
}