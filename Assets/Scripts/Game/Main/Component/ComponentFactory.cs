using Framework;
using Game;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 组件工厂
/// </summary>
public class ComponentFactory : SingletonBase<ComponentFactory>
{
    private ComponentFactory() { }

    /// <summary>
    /// 批量添加组件
    /// </summary>
    /// <param name="character"></param>
    /// <param name="componentIds"></param>
    /// <returns></returns>
    public IDictionary<Type, Component> AddComponents(IEntityObject character, IEnumerable<int> componentIds)
    {
        IDictionary<Type, Component> components = new Dictionary<Type, Component>();

        foreach (int id in componentIds)
        {
            // 根据id创建不同的组件
            switch (id)
            {
                //// 网络移动组件
                //case 1:
                //    AddComponent<NetMoveComponent>(character);
                //    break;
                //// 本地移动组件
                //case 2:
                //    AddComponent<LocalMoveComponent>(character);
                //    break;
                //// 动画组件
                //case 3:
                //    AddComponent<AnimComponent>(character);
                //    break;
                //// 技能组件
                //case 4:
                //    AddComponent<SkillComponent>(character);
                //    break;
                //// 属性组件
                //case 5:
                //    AddComponent<PropertyComponent>(character);
                //    break;
                //// UI组件
                //case 6:
                //    AddComponent<UIComponent>(character);
                //    break;
                default:
                    LogMgr.LogError($"未知的组件ID: {id}");
                    break;
            }
        }
        return components;
    }

    /// <summary>
    /// 添加单个组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="character"></param>
    /// <returns></returns>
    public T AddComponent<T>(IEntityObject entityObject) where T : Component
    {
        T component = entityObject.GameObject.AddComponent<T>();
        if (component is IComponent ic)
        {
            ic.Init(entityObject);
        }
        return component;
    }
}
