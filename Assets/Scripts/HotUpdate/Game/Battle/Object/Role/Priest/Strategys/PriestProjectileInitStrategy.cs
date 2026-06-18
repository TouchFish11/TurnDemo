using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Strategys
{
    public class PriestProjectileInitStrategy : ProjectileInitStrategy
    {
        public void NormalSkillInit(SkillContext skillContext)
        {
            // 初始化投射物核心数据（施法者、主目标、所有目标、当前技能实例）
            skillContext.ProjectileData = new ProjectileData(skillContext.Caster, skillContext.MainTarget, skillContext.AllTargets, this);
            // 初始化投射物变换信息（位置为目标物体位置，旋转为默认）
            skillContext.ProjectileTrans = new ProjectileTrans(skillContext.MainTarget.GameObject.transform.position, Quaternion.identity);
            // 初始化特效信息对象
            skillContext.VFXInfo = poolManager.GetData<VFXInfo>();
        }
        
        public void BattleSkillInit(SkillContext skillContext)
        {
            // 初始化投射物核心数据（关联施法者、目标、技能本身）
            skillContext.ProjectileData = new ProjectileData(skillContext.Caster, skillContext.MainTarget, skillContext.AllTargets, this);
            // 初始化投射物位置（以主目标的位置为基准，旋转为默认）
            skillContext.ProjectileTrans = new ProjectileTrans(skillContext.MainTarget.GameObject.transform.position, Quaternion.identity);
            // 初始化特效信息容器（用于记录特效的生命周期等状态）
            skillContext.VFXInfo = poolManager.GetData<VFXInfo>();
        }
    }
}
