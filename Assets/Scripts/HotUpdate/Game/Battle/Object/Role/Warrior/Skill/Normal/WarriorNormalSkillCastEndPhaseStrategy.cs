using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Normal
{
    public class WarriorNormalSkillCastEndPhaseStrategy : SkillCastEndPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            Logger.LogDebug(TODO, LastAnimationName);
            // 等待动画播放和特效结束
            yield return BattleAnimationComponent.WaitForPlay(LastAnimationName);
            yield return new WaitUntil(() => !SkillContext.VFXInfo.IsAlive);
            // 重置角色本地位置（防止动画位移残留）
            BattleAnimationComponent.Animator.transform.localPosition = Vector3.zero;
            yield return SkillHelper.Delay(100);
        }
    }
}
