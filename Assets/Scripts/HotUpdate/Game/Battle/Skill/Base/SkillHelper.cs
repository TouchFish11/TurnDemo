using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event.Skill;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Property;
using HotUpdate.Game.Battle.Skill.Component;
using HotUpdate.Game.VFX;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Skill.Base
{
    /// <summary>
    /// 技能辅助器
    /// </summary>
    public static class SkillHelper
    {
        private static readonly Dictionary<int, WaitForSeconds> _waitForSecondsMap = new();
        
        /// <summary>
        /// 初始化技能目标，调用战斗协调器设置目标并初始化技能目标
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="battleCoordinator"></param>
        public static void InitRoleSkillTarget(ISkill skill, BattleCoordinator battleCoordinator)
        {
            battleCoordinator.InitSkillTarget(skill);
        }

        /// <summary>
        /// 初始化怪物技能目标
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="battleCoordinator"></param>
        public static void InitMonsterSkillTarget(ISkill skill, BattleCoordinator battleCoordinator)
        {
            var skillContext = skill.SkillContext;
            // 根据技能配置和选择策略，筛选出技能作用的目标
            battleCoordinator.SetSelectSkillInfo(skillContext.SkillInfo);
            battleCoordinator.SelectTargets(skillContext.Caster, skillContext.TargetSelectStrategy);
            battleCoordinator.InitSkillTarget(skill);
        }

        /// <summary>
        /// 等待动画播放到指定动画的目标进度
        /// </summary>
        /// <param name="context"></param>
        /// <param name="layerName"></param>
        /// <param name="stateName"></param>
        /// <param name="targetEndProgress"></param>
        /// <returns></returns>
        public static IEnumerator WaitForAnimationPlayTarget(SkillContext context, string layerName, string stateName, float targetEndProgress)
        {
            // 获取施法者的动画组件
            var animationComponent = context.Caster.GetComponent<BattleAnimationComponent>();
            // 根据配置表设置技能对应的动画状态
            animationComponent.SetAnimationState(context.SkillInfo.f_animationType);
            // 等待动画播放到指定状态
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(layerName).IsName(stateName));
            // 等待动画播放至目标进度
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(layerName).normalizedTime >= targetEndProgress);
        }
        
        /// <summary>
        /// 等待玩家终结技确认
        /// </summary>
        /// <param name="skillContext"></param>
        /// <returns></returns>
        public static IEnumerator WaitForUltimateConfirm(SkillContext skillContext)
        {
            var skillComponent = skillContext.Caster.GetComponent<PlayerSkillComponent>();
            // 待技能组件确认释放（阻塞直到释放条件满足）
            yield return new WaitUntil(() => skillComponent.IsRelease);
            // 清空释放者当前能量（终结技消耗所有能量）
            skillContext.PropertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentEnergy, 0);
            // 终结释放通用逻辑、禁用输入、更新UI显示
            var context = skillContext.Caster.Context;
            context.GetEventBus().TriggerEvent(new UltimateCastEvent(context));
        }

        /// <summary>
        /// 延迟协程
        /// </summary>
        /// <param name="delayMs">延迟时间，单位毫秒</param>
        /// <returns></returns>
        public static IEnumerator Delay(int delayMs)
        {
            if (_waitForSecondsMap.TryGetValue(delayMs, out var waitForSeconds))
            {
                yield return waitForSeconds;
            }
            else
            {
                var newSeconds = new WaitForSeconds(delayMs / 1000f);
                _waitForSecondsMap.Add(delayMs, newSeconds);
                yield return newSeconds;
            }
        }

        /// <summary>
        /// 创建特效任务转换为协程，并在完成初始化技能上下文的弹射物属性
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vfxCreateTask"></param>
        /// <returns></returns>
        public static IEnumerator WaitForCreateVFX(SkillContext context, Task<IProjectile> vfxCreateTask)
        {
            yield return TaskUtility.WaitForTask(vfxCreateTask, projectile => context.Projectile = projectile);
            yield return new WaitUntil(() => context.Projectile != null);
        }
        
        public static void PrintSelectTargets(List<IBattleEntityObject> allTargets)
        {
            // 拼接并打印所有目标信息（调试用）
            var sb = new StringBuilder();
            sb.AppendJoin('、', allTargets);
            Logger.Log($"怪物选择目标: {sb}");
        }
    }
}
