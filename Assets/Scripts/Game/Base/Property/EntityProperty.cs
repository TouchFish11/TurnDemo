using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 实体属性
/// </summary>
public abstract class EntityProperty
{
    protected int id;

    /// <summary>
    /// 初始化属性
    /// </summary>
    public abstract void InitProperty(int id);

    public int Id => id;
}
