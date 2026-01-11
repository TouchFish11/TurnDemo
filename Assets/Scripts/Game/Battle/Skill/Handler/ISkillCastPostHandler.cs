using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能释放后处理器
/// </summary>
public interface ISkillCastPostHandler
{
    /// <summary>
    /// 处理
    /// </summary>
    /// <returns></returns>
    IEnumerator OnHnadle(ISkill skill);
}
