using UnityEngine;

namespace HotUpdate.Base.ECModule
{
    /// <summary>
    /// 游戏实体对象核心接口
    /// 定义所有游戏实体的基础行为和属性规范
    /// </summary>
    public interface IEntityObject
    {
        /// <summary>
        /// 实体对象唯一ID
        /// </summary>
        long EntityId { get; }
        
        /// <summary>
        /// 当前实体绑定的GameObject
        /// </summary>
        GameObject GameObject { get; }

        /// <summary>
        /// 实体基础初始化方法，用于初始化实体的基础信息、绑定核心组件等，只能通过EntityHelper初始化调用
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="service"></param>
        void InitBase(long entityId, ComponentService service);

        /// <summary>
        /// 获取实体身上指定类型的组件
        /// 仅返回实现了IComponent接口的Unity组件
        /// </summary>
        /// <typeparam name="T">要获取的组件类型，需继承UnityEngine.Component并实现IComponent</typeparam>
        /// <returns>匹配类型的组件实例，若无则返回null</returns>
        T GetComponent<T>() where T : IComponent;

        /// <summary>
        /// 递归获取实体子对象中指定类型的组件
        /// 仅返回实现了IComponent接口的Unity组件
        /// </summary>
        /// <typeparam name="TComponent">要获取的组件类型，需继承UnityEngine.Component并实现IComponent</typeparam>
        /// <returns>匹配类型的组件实例，若无则返回null</returns>
        TComponent GetComponentInChildren<TComponent>() where TComponent : IComponent;

        /// <summary>
        /// 为实体添加指定类型的组件
        /// 仅添加实现了IComponent接口的Unity组件
        /// </summary>
        /// <typeparam name="TComponent">要添加的组件类型，需继承UnityEngine.Component并实现IComponent</typeparam>
        /// <returns>新增的组件实例</returns>
        TComponent AddComponent<TComponent>() where TComponent : Component, IComponent;

        /// <summary>
        /// 批量添加组件（按组件名称）
        /// 组件名称需与类名一致，且需继承UnityEngine.Component并实现IComponent
        /// </summary>
        /// <param name="componentNames">要添加的组件名称数组</param>
        /// <returns>添加结果：true表示全部添加成功，false表示至少有一个组件添加失败</returns>
        bool AddComponents(params string[] componentNames);

        /// <summary>
        /// 销毁实体
        /// 用于释放实体资源、解绑事件、销毁Unity游戏对象等清理操作
        /// </summary>
        void Destroy();
    }
}