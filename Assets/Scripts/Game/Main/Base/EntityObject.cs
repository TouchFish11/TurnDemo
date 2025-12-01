using GameLogic.BattleMoudule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityObject : MonoBehaviour, IEntityObject
{
    // 组件类型映射
    private readonly Dictionary<Type, IComponent> typeToComponentMap = new Dictionary<Type, IComponent>();

    // 组件配置
    [SerializeField] private ComponentConfig componentConfig;

    public GameObject GameObject => this.gameObject;

    public virtual void Init(int id)
    {
        // 初始化组件
        ComponentFactory.Instance.AddComponents(this, componentConfig.compnentIds);
    }

    public new TComponent GetComponent<TComponent>() where TComponent : MonoBehaviour
    {
        return typeToComponentMap[typeof(TComponent)] as TComponent;
    }

    public TComponent AddComponent<TComponent>() where TComponent : MonoBehaviour
    {
        return ComponentFactory.Instance.AddComponent<TComponent>(this);
    }
}
