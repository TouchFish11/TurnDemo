using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画参数
/// </summary>
public class AnimationParameter
{
    /// <summary>
    /// 移动参数hash（bool）
    /// </summary>
    public int IsRunHash { get; } = Animator.StringToHash("IsRun");

    /// <summary>
    /// 普攻预备释放hash
    /// </summary>
    public int PreNormalAttackTriggerHash { get; } = Animator.StringToHash("PreNormalAttackTrigger");

    /// <summary>
    /// 普通攻击触发hash
    /// </summary>
    public int NormalAtkTirggerHash { get; } = Animator.StringToHash("NormalAtkTirgger");

    /// <summary>
    /// 战技预备释放hash
    /// </summary>
    public int PreBattleAttackTriggerHash { get; } = Animator.StringToHash("PreBattleAttackTrigger");

    /// <summary>
    /// 战技攻击触发hash
    /// </summary>
    public int BattleAtkTriggerHash { get; } = Animator.StringToHash("BattleAtkTrigger");

    /// <summary>
    /// 终结技预备释放hash
    /// </summary>
    public int PreUltimateAttackTriggerHash { get; } = Animator.StringToHash("PreUltimateAttackTrigger");

    /// <summary>
    /// 终结技攻击触发hash
    /// </summary>
    public int UltimateAtkTriggerHash { get; } = Animator.StringToHash("UltimateAtkTrigger");

    /// <summary>
    /// 受击触发hash
    /// </summary>
    public int HitTriggerHash { get; } = Animator.StringToHash("HitTrigger");

    /// <summary>
    /// 死亡触发hash
    /// </summary>
    public int DeathTriggerHash { get; } = Animator.StringToHash("DeathTrigger");

    /// <summary>
    /// 重生触发hash
    /// </summary>
    public int RebirthTriggerHash { get; } = Animator.StringToHash("RebirthTrigger");
}
