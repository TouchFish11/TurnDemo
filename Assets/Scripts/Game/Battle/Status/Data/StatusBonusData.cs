using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态加成数据
/// 每个状态内部加成
/// </summary>
public struct StatusBonusData
{
    /// <summary>
    /// 攻击力百分比加成属性
    /// </summary>
    public int AtkPercentBonus { get; set; }

    /// <summary>
    /// 攻击力固定数值加成属性
    /// </summary>
    public int AtkBuildBonus { get; set; }

    /// <summary>
    /// 防御力百分比加成属性
    /// </summary>
    public int DefPercentBonus { get; set; }

    /// <summary>
    /// 防御力固定数值加成属性
    /// </summary>
    public int DefBuildBonus { get; set; }

    /// <summary>
    /// 降低防御力百分比
    /// </summary>
    public int SubDefPercent { get; set; }

    /// <summary>
    /// 无视防御百分比
    /// </summary>
    public int IgnoreDefPercent { get; set; }

    /// <summary>
    /// 生命值百分比加成属性
    /// </summary>
    public int HpPercentBonus { get; set; }

    /// <summary>
    /// 生命值固定数值加成属性
    /// </summary>
    public int HpBuildBonus { get; set; }
}
