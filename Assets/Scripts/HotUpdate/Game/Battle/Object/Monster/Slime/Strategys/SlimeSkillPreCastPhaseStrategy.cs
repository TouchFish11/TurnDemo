using System.Collections;
using System.Text;
using Core.Utility;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.VFX;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Object.Monster.Slime.Strategys
{
    public class SlimeSkillPreCastPhaseStrategy : SkillPreCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            // 根据技能配置和选择策略，筛选出技能作用的目标
            battleCoordinator.SetSelectSkillInfo(SkillContext.SkillInfo);
            battleCoordinator.SelectTargets(SkillContext.Caster, SkillContext.TargetSelectStrategy);
            // TODO；暂时这样写
            battleCoordinator.InitSkillTarget(skill);
            
            // 获取主目标位置（仅保留XZ平面，忽略Y轴高度）
            var mainTarget = SkillContext.MainTarget.GameObject.transform.position;
            mainTarget = new Vector3(mainTarget.x, 0, mainTarget.z);
            // 获取施法者位置（仅保留XZ平面）
            var caster = SkillContext.Caster.GameObject.transform.position;
            caster = new Vector3(caster.x, 0, caster.z);
            
            // 初始化投射物数据（施法者、主目标、所有目标、当前技能）
            SkillContext.ProjectileData = new ProjectileData(SkillContext.Caster, SkillContext.MainTarget, SkillContext.AllTargets, skill);
            // 初始化技能弹道的位置（施法者前方）和朝向（面向主目标）
            SkillContext.ProjectileTrans = new ProjectileTrans(SkillContext.Caster.GameObject.transform.position + Vector3.forward, Quaternion.LookRotation(mainTarget - caster));
            // 初始化特效信息对象
            SkillContext.VFXInfo = poolManager.GetData<VFXInfo>();
            
            // 拼接并打印所有目标信息（调试用）
            var sb = new StringBuilder();
            foreach (var battleEntityObject in SkillContext.AllTargets)
            {
                sb.AppendLine($"怪物选择目标：{battleEntityObject}");
            }
            Logger.Log($"{sb}");
            
            yield return TaskUtility.WaitForTask(battleCoordinator.UpdateCamera((PlayerObject)SkillContext.MainTarget));

            yield return new WaitForSeconds(0.1f);
        }
    }
}
