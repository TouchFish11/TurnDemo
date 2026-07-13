using System.Collections;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Skill.Battle
{
    public class PriestBattleSkillPreCastPhaseStrategy : SkillPreCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            SkillHelper.InitRoleSkillTarget(skill, battleCoordinator);
            
            var skillContext = skill.SkillContext;
            
            // 消耗战斗点数（BP），消耗数值取自技能配置表的f_costBP字段
            skillContext.Caster.Context.ConsumeSkillPoint(skillContext.SkillInfo.f_costBP);
            
            skillContext.ProjectileData = new ProjectileData(skillContext.Caster, skillContext.MainTarget, skillContext.AllTargets, SkillContext);
            skillContext.ProjectileTrans = new ProjectileTrans(skillContext.MainTarget.GameObject.transform.position, Quaternion.identity);
            skillContext.VFXInfo = poolManager.GetData<VFXInfo>();
            
            var context = skill.SkillContext.Caster.Context;
            context.EventBus.TriggerEvent(new PlayerReleaseSkillEvent(context));
            yield break;
        }
    }
}
