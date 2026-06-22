using HotUpdate.Base.Object;

namespace HotUpdate.Base.Component
{
    /// <summary>
    /// 组件核心接口，定义所有游戏组件的基础行为规范
    /// 所有具体的游戏组件都应实现此接口
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// 获取当前组件所属的实体对象
        /// 实体对象是组件的载体，一个实体可挂载多个不同类型的组件
        /// </summary>
        IEntityObject EntityObject { get; }

        /// <summary>
        /// 初始化组件
        /// 在组件挂载到实体对象时调用，用于完成组件的初始状态设置、依赖绑定等
        /// </summary>
        /// <param name="entityObject">当前组件要归属的实体对象</param>
        void Init(IEntityObject entityObject);
        
        /// <summary>
        /// 组件销毁方法
        /// 用于释放组件持有的资源，默认清空实体对象引用，子类可重写扩展销毁逻辑
        /// </summary>
        /// <remarks>
        /// 建议在实体销毁时调用，而非仅依赖 MonoBehaviour.OnDestroy
        /// 避免 GameObject 销毁时的资源释放时机问题
        /// </remarks>
        void Destroy();
    }
}