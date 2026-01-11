using Game;
using Game.Battle;
using System.Collections.Generic;

/// <summary>
/// 玩家基础目标选择策略
/// </summary>
public class PlayerBaseTargetSelectStrategy : ITargetSelectStrategy
{
    public IBattleEntityObject SelectMainTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo)
    {
        // 获取技能目标类型
        E_SkillTargetType targetType = (E_SkillTargetType)skillInfo.f_targetType;
        // 根据技能目标类型获取所有敌方/友方实体

        List<IBattleEntityObject> targets = null;
        if (caster is PlayerObject)
        {
            targets = new List<IBattleEntityObject>(targetType == E_SkillTargetType.Enemy ? context.GetMonsterObjects() : context.GetPlayerObjects());
        }
        else if (caster is MonsterObject)
        {
            targets = new List<IBattleEntityObject>(targetType == E_SkillTargetType.Enemy ? context.GetPlayerObjects() : context.GetMonsterObjects());
        }

        IBattleEntityObject currentMainTarget = null;
        // 若当前目标为空且当前选中的目标已经死亡，则需要重新选择目标；否则就默认选中上次选中的目标
        while (currentMainTarget == null || currentMainTarget.GetComponent<PropertyComponent>().IsDeath)
        {
            int targetNum = targets.Count;
            // 若没有目标，则不用选择，返回空
            if (targetNum == 0)
            {
                return null;
            }
            // 若只有一个目标，则默认选择该目标
            else if (targetNum == 1)
            {
                currentMainTarget = targets[0];
            }
            // 若有多个目标，则默认选择靠近中间的目标
            else
            {
                // 奇数数量，选中中间目标；偶数数量，选中右边目标
                currentMainTarget = targets[targetNum / 2];
            }
        }
        // 返回主目标
        return currentMainTarget;
    }
}
