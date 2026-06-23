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

        protected IComponentCore<IComponent> ComponentCore { get; private set; }

        /// <summary>
        /// 自动获取挂载同一 GameObject 下的 <see cref="IEntityObject"/> 组件，完成实体关联
        /// </summary>
        private void Awake()
        {
            EntityObject = GetComponent<IEntityObject>();
        }
        
        void IComponent.Init(IEntityObject entityObject, IComponentCore<IComponent> componentCore)
        {
            EntityObject = entityObject;
            ComponentCore = componentCore;
            OnInit();
        }

        protected virtual void OnInit()
        {
            
        }

        public void Destroy()
        {
            OnDestroyBase();
            ComponentCore.Dispose();
            ComponentCore = null;
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