using Core.DI;
using HotUpdate.Base.Object;
using UnityEngine;

namespace HotUpdate.Base.Component
{
    /// <summary>
    /// 所有实体组件的基类，定义组件的核心生命周期与实体关联逻辑
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
        /// 自动获取挂载同一 GameObject 下的 <see cref="IEntityObject"/> 组件，完成实体关联
        /// </summary>
        private void Awake()
        {
            EntityObject = GetComponent<IEntityObject>();
            DIContainer.InjectIntoInstance(this);
        }

        /// <summary>
        /// 组件初始化方法（抽象）
        /// 需由子类实现具体的初始化逻辑，用于接收外部实体对象并完成组件初始化
        /// </summary>
        /// <param name="entityObject">当前组件所属的实体对象</param>
        public abstract void Init(IEntityObject entityObject);

        public void Destroy()
        {
            OnDestroyBase();
            EntityObject = null;
        }

        /// <summary>
        /// 组件基础销毁
        /// </summary>
        protected virtual void OnDestroyBase()
        {
            
        }
    }
}