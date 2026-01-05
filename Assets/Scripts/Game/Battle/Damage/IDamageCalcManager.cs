using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 伤害计算管理器接口
/// </summary>
public interface IDamageCalcManager
{
    void CalcSkillDamage(IBattleEntityObject source, IBattleEntityObject target, SkillInfo skillInfo, out DamageResult damageResult);
}
