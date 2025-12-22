using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗属性
/// </summary>
public abstract class BattleProperty : EntityProperty
{
    // 基础属性
    protected int baseHp;   // 生命值
    protected int baseAtk;  // 攻击力
    protected int baseDef;  // 防御力

    // 进阶属性
    protected int baseSpeed;    // 速度
    // ...

    // 动态属性
    protected int currentHp;    // 当前生命值
    protected int maxHp;    // 最大生命值
    protected int maxAtk;   // 最大攻击力
    protected int maxDef;   // 最大防御力
    protected int currentSpeed;     // 当前速度

    public int BaseHp => baseHp;
    public int BaseAtk => baseAtk;
    public int BaseDef => baseDef;
    public int BaseSpeed => baseSpeed;

    public int CurrentHp { get => currentHp; set => currentHp = value; }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int MaxAtk { get => maxAtk; set => maxAtk = value; }
    public int MaxDef { get => maxDef; set => maxDef = value; }
    public int CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }
}
