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
            
            // 初始化投射物核心数据（关联施法者、目标、技能本身）
            skillContext.ProjectileData = new ProjectileData(skillContext.Caster, skillContext.MainTarget, skillContext.AllTargets, SkillContext);
            // 初始化投射物位置（以主目标的位置为基准，旋转为默认）
            skillContext.ProjectileTrans = new ProjectileTrans(skillContext.MainTarget.GameObject.transform.position, Quaternion.identity);
            // 初始化特效信息容器（用于记录特效的生命周期等状态）
            skillContext.VFXInfo = poolManager.GetData<VFXInfo>();
            
            var context = skill.SkillContext.Caster.Context;
            context.GetEventBus().TriggerEvent(new PlayerReleaseSkillEvent(context));
            yield break;
        }
    }
}
