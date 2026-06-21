using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.Ashfall
{
    public class AbyssalMageAshfallSkillCastEndPhaseStrategy : SkillCastEndPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            // 等待第二段VFX结束
            yield return new WaitUntil(() => !SkillContext.VFXInfo.IsAlive);
            // 技能结束前短暂延迟
            yield return SkillHelper.Delay(200);
        }
    }
}
