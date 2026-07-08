using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Battle
{
    public class WarriorBattleSkillCastEndPhaseStrategy : SkillCastEndPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            // 等待动画播放和特效结束
            yield return BattleAnimationComponent.WaitForPlay(LastAnimationName);
            yield return new WaitUntil(() => !SkillContext.VFXInfo.IsAlive);
            yield return SkillHelper.Delay(100);
        }
    }
}
