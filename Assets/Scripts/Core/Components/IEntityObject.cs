using UnityEngine;

namespace Core.Components
{
    /// <summary>
    /// 游戏实体对象核心接口
    /// 定义所有游戏实体的基础行为和属性规范
    /// </summary>
    public interface IEntityObject
    {
        /// <summary>
        /// 实体对应的Unity游戏对象
        /// 用于挂载组件、控制显示/位置等Unity原生操作
        /// </summary>
        GameObject GameObject { get; }

        /// <summary>
        /// 实体属性组件
        /// 存储实体的核心属性，由子类初始化并维护
        /// </summary>
        EntityProperty EntityProperty { get; }

        /// <summary>
        /// 实体基础初始化方法
        /// 用于初始化实体的基础信息、绑定核心组件等
        /// </summary>
        /// <param name="id">实体唯一标识ID，用于区分不同实体实例</param>
        void BaseInit(int id);

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