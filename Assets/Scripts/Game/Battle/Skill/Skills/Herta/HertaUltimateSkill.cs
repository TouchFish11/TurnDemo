using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HertaUltimateSkill : UltimateSkill
{
    // 测试：终结三段造成伤害时机
    private float[] dmgTimes = new float[] { 0.03f, 0.06f, 0.1f };

    protected override int DmgCount { get; set; } = 3;

    public HertaUltimateSkill(int skillId, ISkillCastPostHandler postHandler) : base(skillId, postHandler)
    {

    }

    protected override void OnPreUltimateCast(IBattleContext context)
    {
        base.OnPreUltimateCast(context);

        // 播放预备动画：玩家终结技pose、终结技动画
        Caster.GetComponent<BattleAnimationComponent>().SetUltimatePose(); 
    }

    protected override IEnumerator OnUltimateCast(IBattleContext context)
    {
        // TODO：暂时直接触发对应动画，之后根据具体技能的时机触发
        context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));

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

                RecoverEnergy();
                LogManager.Log($"【终结技】：{Caster.GameObject.name}释放技能：{SkillInfo.f_name}，第{index + 1}段");
                index++;
            }

            yield return null;
        }
    }
}
