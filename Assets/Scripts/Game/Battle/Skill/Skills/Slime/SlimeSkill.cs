using Framework;
using Game;
using Game.Battle;
using System.Collections;
using UnityEngine;

/// <summary>
/// 史莱姆技能
/// </summary>
public class SlimeSkill : MonsterSkill
{
    private static WaitForSeconds _waitForSeconds0_3 = new WaitForSeconds(0.3f);

    private readonly float moveSpeed = 15f;
    private readonly float dis = 1f;

    /// <summary>
    /// 攻击
    /// 目前是怪物使用
    /// </summary>
    public string Attack { get; } = "Attack";
    protected override int DmgCount { get; set; } = 1;

    public SlimeSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
    {

    }

    private void OnAttack(float time)
    {
        // 处理伤害
        foreach (IBattleEntityObject battleEntity in AllTargets)
        {
            DamageCalcManager.CalcSkillDamage(Caster, battleEntity, this.SkillInfo, out DamageResult result);
            battleEntity.TakeDamage(result);
        }
        StatusAddStrategy?.ToAdd(Caster, AllTargets, statusIds);
    }

    protected override IEnumerator OnCast(IBattleContext context)
    {
        Vector3 targetPos = MainTarget.GameObject.transform.position;
        while (Vector3.Distance(Caster.GameObject.transform.position, targetPos) > dis)
        {
            Vector3 nowPos = Caster.GameObject.transform.position;
            Caster.GameObject.transform.position = Vector3.MoveTowards(nowPos, targetPos, Time.deltaTime * moveSpeed);
            yield return null;
        }

        yield return AnimationPlayManager.Instance.PlayAnimation(Caster, (E_AnimationType)SkillInfo.f_animationType, Attack, OnAttack, TextUtility.SplitTofloatArr(SkillInfo.f_dmgTimes, 2));

        // 优化表现
        yield return _waitForSeconds0_3; 

        // 回到起始位置
        targetPos = BattlePoint.Instance.GetMonsterTransByIndex(Caster.EntityPosIndex).position;
        while (Vector3.Distance(Caster.GameObject.transform.position, targetPos) >= 0.1f)
        {
            Vector3 nowPos = Caster.GameObject.transform.position;
            Caster.GameObject.transform.position = Vector3.MoveTowards(nowPos, targetPos, Time.deltaTime * moveSpeed);
            yield return null;
        }

        Caster.GameObject.transform.position = targetPos;
    }
}
