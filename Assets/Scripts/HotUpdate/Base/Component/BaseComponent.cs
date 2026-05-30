using Core.Components;
using Core.DI;
using UnityEngine;

namespace HotUpdate.Base.Component
{
    /// <summary>
    /// 所有实体组件的基类，定义组件的核心生命周期与实体关联逻辑
    /// 抽象类，需由具体组件继承并实现抽象方法
    /// </summary>
    /// <remarks>
    /// 实现 <see cref="IComponent"/> 接口，遵循组件统一规范
    /// </remarks>
    public abstract class BaseComponent : MonoBehaviour, IComponent
    {
        /// <summary>
        /// 当前组件所属的实体对象
        /// 仅在 Awake 阶段初始化，外部只读，内部私有赋值
        /// </summary>
        public IEntityObject EntityObject { get; private set; }

        /// <summary>
        /// Unity 生命周期 - 唤醒阶段
        /// 自动获取挂载同一 GameObject 下的 <see cref="IEntityObject"/> 组件，完成实体关联
        /// </summary>
        /// <remarks>
        /// 执行时机早于 Start，确保组件初始化前已关联实体
        /// </remarks>
        private void Awake()
        {
            DIContainer.InjectIntoInstance(this);
            EntityObject = GetComponent<IEntityObject>();
        }

        /// <summary>
        /// 组件初始化方法（抽象）
        /// 需由子类实现具体的初始化逻辑，用于接收外部实体对象并完成组件初始化
        /// </summary>
        /// <param name="entityObject">当前组件所属的实体对象</param>
        public abstract void Init(IEntityObject entityObject);

        /// <summary>
        /// 组件销毁方法（虚方法）
        /// 用于释放组件持有的资源，默认清空实体对象引用，子类可重写扩展销毁逻辑
        /// </summary>
        /// <remarks>
        /// 建议在实体销毁时调用，而非仅依赖 MonoBehaviour.OnDestroy
        /// 避免 GameObject 销毁时的资源释放时机问题
        /// </remarks>
        public virtual void Destroy()
        {
            EntityObject = null;
        }
    }
}