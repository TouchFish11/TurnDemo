using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能键位UI数据
/// </summary>
public readonly struct SkillKeyUIData
{
    public List<SkillInfo> SkillInfos { get; }

    public IBattleEntityObject Provider { get; }

    public SkillKeyUIData(List<SkillInfo> skillInfos, IBattleEntityObject provider)
    {
        SkillInfos = skillInfos;
        Provider = provider;
    }
}

/// <summary>
/// 技能按键UI数据提供器
/// </summary>
public interface ISkillKeyUIDataProvider
{
    /// <summary>
    /// 获取数据
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    SkillKeyUIData GetData(IBattleEntityObject provider);
}
