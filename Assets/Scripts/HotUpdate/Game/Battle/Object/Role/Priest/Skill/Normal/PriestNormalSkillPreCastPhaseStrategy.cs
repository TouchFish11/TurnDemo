using System.Collections;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Skill.Normal
{
    public class PriestNormalSkillPreCastPhaseStrategy : SkillPreCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            SkillHelper.InitRoleSkillTarget(skill, battleCoordinator);
            
            var skillContext = skill.SkillContext;
            
            // 消耗战斗点数（BP），消耗数值取自技能配置表的f_costBP字段
            skillContext.Caster.Context.ConsumeSkillPoint(skillContext.SkillInfo.f_costBP);
            
            // 初始化投射物核心数据（施法者、主目标、所有目标、当前技能实例）
            skillContext.ProjectileData = new ProjectileData(skillContext.Caster, skillContext.MainTarget, skillContext.AllTargets, SkillContext);
            // 初始化投射物变换信息（位置为目标物体位置，旋转为默认）
            skillContext.ProjectileTrans = new ProjectileTrans(skillContext.MainTarget.GameObject.transform.position, Quaternion.identity);
            // 初始化特效信息对象
            skillContext.VFXInfo = poolManager.GetData<VFXInfo>();
            
            var context = skill.SkillContext.Caster.Context;
            context.EventBus.TriggerEvent(new PlayerReleaseSkillEvent(context));
            yield break;
        }
    }
}
