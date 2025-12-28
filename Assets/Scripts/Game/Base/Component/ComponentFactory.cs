using Framework;
using Game;
using Game.Battle;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 组件工厂
/// </summary>
public class ComponentFactory
{
    /// <summary>
    /// 批量添加组件
    /// </summary>
    /// <param name="entityObject"></param>
    /// <param name="componentIds"></param>
    /// <returns></returns>
    public static IDictionary<Type, Component> AddComponents(IEntityObject entityObject, IEnumerable<int> componentIds)
    {
        IDictionary<Type, Component> components = new Dictionary<Type, Component>();

        foreach (int id in componentIds)
        {
            // 根据id创建不同的组件
            switch (id)
            {
                // 动画组件
                case 1:
                    AddComponent<AnimComponent>(entityObject);
                    break;
                // 技能组件
                case 2:
                    AddComponent<SkillComponent>(entityObject);
                    break;
                //// 玩家角色组件
                //case 3:
                //    AddComponent<PlayerObject>(entityObject);
                //    break;
                //// 怪物角色组件
                //case 4:
                //    AddComponent<MonsterObject>(entityObject);
                //    break;
                // 网络移动组件
                case 5:
                    //AddComponent<NetMoveComponent>(entityObject);
                    break;
                // 本地移动组件
                case 6:
                    //AddComponent<LocalMoveComponent>(entityObject);
                    break;
                // 玩家属性组件
                case 7:
                    AddComponent<PlayerPropertyComponent>(entityObject);
                    break;
                // 怪物属性组件
                case 8:
                    AddComponent<MonsterPropertyComponent>(entityObject);
                    break;
                default:
                    LogManager.LogError($"未知的组件ID: {id}");
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
    public static T AddComponent<T>(IEntityObject entityObject) where T : Component
    {
        T component = entityObject.GameObject.AddComponent<T>();
        if (component is IComponent ic)
        {
            ic.Init(entityObject);
        }
        return component;
    }
}
