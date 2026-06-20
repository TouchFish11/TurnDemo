using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill.Battle
{
    public class WizardBattleSkillPreCastPhaseStrategy : SkillPreCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            SkillHelper.InitSkillTarget(skill, battleCoordinator);
            
            // 初始化投射物核心数据（释放者、主目标、所有目标、当前技能实例）
            SkillContext.ProjectileData = new ProjectileData(SkillContext.Caster, SkillContext.MainTarget, SkillContext.AllTargets, this);
            // 初始化投射物变换信息（位置为目标位置，旋转默认）
            SkillContext.ProjectileTrans = new ProjectileTrans(SkillContext.MainTarget.GameObject.transform.position, Quaternion.identity);
            // 初始化特效信息容器
            SkillContext.VFXInfo = poolManager.GetData<VFXInfo>();
            yield break;
        }
    }
}
