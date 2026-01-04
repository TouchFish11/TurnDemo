using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HertaUltimateSkill : UltimateSkill
{
    // 测试：终结三段造成伤害时机
    private float[] dmgTimes = new float[] { 0.03f, 0.06f, 0.1f };

    public HertaUltimateSkill(int skillId) : base(skillId)
    {

    }

    protected override IEnumerator OnUltimateCast(IBattleContext context)
    {
        BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
        int index = 0;
        while (index < dmgTimes.Length)
        {
            AnimatorStateInfo stateInfo = animationComponent.GetCurrentAnimatorStateInfo();
            if (stateInfo.normalizedTime > dmgTimes[index])
            {
                foreach (IBattleEntityObject battleEntity in AllTargets)
                {
                    DamageCalcManager.CalcSkillDamage(Caster, battleEntity, this.SkillInfo, out DamageResult result);
                    battleEntity.TakeDamage(result);
                }
                LogManager.Log($"【终结技】：{Caster.GameObject.name}释放技能：{SkillInfo.f_name}，第{index + 1}段");
                index++;
            }

            yield return null;
        }
    }
}
