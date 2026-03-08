using System.Collections;
using System.Text;
using Core.Log;
using Core.Pool;
using Core.Service;
using HotUpdate.Battle.Skill.Base;
using HotUpdate.Common;
using HotUpdate.Core.Animation;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.VFX;
using UnityEngine;

namespace HotUpdate.Battle.Object.Monster.TurtleShell.Skill
{
    /// <summary>
    /// 甲壳虫技能
    /// </summary>
    public class TurtleShellSkill : MonsterSkill
    {
        /// <summary>
        /// 普攻动画状态名称
        /// 当前仅用于普攻技能的动画判断
        /// </summary>
        public static string Attack => "Attack";

        public TurtleShellSkill(IBattleEntityObject caster, int skillId) : base(caster, skillId)
        {
            Caster.GetComponentInChildren<IAnimationTrigger>().OnAttack += OnAttack;
        }

        private async void OnAttack(int skillId)
        {
            if (skillId != SkillInfo.f_id)
            {
                return;
            }

            await ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_MonsterAttackSkill, projectileTrans, projectileData, vFXInfo);
        }
        
        /// <summary>
        /// 初始化投射物/技能弹道数据（重写基类方法）
        /// 主要设置技能特效的生成位置、朝向，以及打印目标信息
        /// </summary>
        protected override void InitProjectile()
        {
            // 更新战斗相机视角
            Caster.Context.GetProxy().UpdateCamera(MainTarget);
            
            // 获取主目标位置（仅保留XZ平面，忽略Y轴高度）
            var mainTarget = MainTarget.GameObject.transform.position;
            mainTarget = new Vector3(mainTarget.x, 0, mainTarget.z);
            // 获取施法者位置（仅保留XZ平面）
            var caster = Caster.GameObject.transform.position;
            caster = new Vector3(caster.x, 0, caster.z);
            
            // 初始化投射物数据（施法者、主目标、所有目标、当前技能）
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            // 初始化技能弹道的位置（施法者前方）和朝向（面向主目标）
            projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position + Vector3.forward, Quaternion.LookRotation(mainTarget - caster));
            // 初始化特效信息对象
            vFXInfo = ServiceLocator.Get<IPoolManager>().GetData<VFXInfo>();
            
            // 拼接并打印所有目标信息（调试用）
            var sb = new StringBuilder();
            foreach (var battleEntityObject in AllTargets)
            {
                sb.AppendLine($"怪物选择目标：{battleEntityObject}");
            }
            LogManager.Log($"{sb}");
        }

        /// <summary>
        /// 技能释放核心逻辑（重写基类方法）
        /// 处理动画播放、特效等待、技能流程时序
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <returns>协程迭代器</returns>
        protected override IEnumerator OnCast(IBattleContext context)
        {
            // 技能释放前短暂延迟
            yield return new WaitForSeconds(0.1f);
            
            // 获取施法者的动画组件
            var animationComponent = Caster.GetComponent<IBattleAnimationComponent>();
            // 根据配置表设置技能对应的动画状态
            animationComponent.SetAnimationState(SkillInfo.f_animationType);
            
            // 等待动画播放到普攻状态（Attack）
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(Attack));
            
            // 等待动画播放至90%且特效已结束，确保技能流程完整
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);
            
            // 技能结束前短暂延迟
            yield return new WaitForSeconds(0.2f);
        }
    }
}
