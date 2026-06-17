using System.Text;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.VFX;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Object.Monster.Slime.Strategys
{
    public class SlimeProjectileInitStrategy : ProjectileInitStrategy
    {
        public override void Init(SkillContext skillContext)
        {
            // 获取主目标位置（仅保留XZ平面，忽略Y轴高度）
            var mainTarget = skillContext.MainTarget.GameObject.transform.position;
            mainTarget = new Vector3(mainTarget.x, 0, mainTarget.z);
            // 获取施法者位置（仅保留XZ平面）
            var caster = skillContext.Caster.GameObject.transform.position;
            caster = new Vector3(caster.x, 0, caster.z);
            
            // 初始化投射物数据（施法者、主目标、所有目标、当前技能）
            skillContext.ProjectileData = new ProjectileData(skillContext.Caster, skillContext.MainTarget, skillContext.AllTargets, skill);
            // 初始化技能弹道的位置（施法者前方）和朝向（面向主目标）
            skillContext.ProjectileTrans = new ProjectileTrans(skillContext.Caster.GameObject.transform.position + Vector3.forward, Quaternion.LookRotation(mainTarget - caster));
            // 初始化特效信息对象
            skillContext.VFXInfo = poolManager.GetData<VFXInfo>();
            
            // 拼接并打印所有目标信息（调试用）
            var sb = new StringBuilder();
            foreach (var battleEntityObject in skillContext.AllTargets)
            {
                sb.AppendLine($"怪物选择目标：{battleEntityObject}");
            }
            Logger.Log($"{sb}");
        }
    }
}
