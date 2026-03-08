using System.Collections;
using Core.Pool;
using Core.Service;
using Core.UI;
using Core.Utility;
using HotUpdate.Battle.Skill.Base;
using HotUpdate.Battle.UI.Base;
using HotUpdate.Common;
using HotUpdate.Core.Animation;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Layer;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Camera;
using HotUpdate.Core.VFX;
using UnityEngine;

namespace HotUpdate.Battle.Object.Role.Priest.Skill
{
    /// <summary>
    /// 牧师终结技
    /// </summary>
    public class PriestUltimateSkill : UltimateSkill
    {
        private const string Priest_Ultimate_01 = nameof(Priest_Ultimate_01);
        private const string Priest_Ultimate_02 = nameof(Priest_Ultimate_02);
        
        public PriestUltimateSkill(IBattleEntityObject caster, int skillId) : base(caster, skillId)
        {
        }

        protected override async void InitProjectileAndPoseVfx()
        {
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position, Quaternion.identity);
            vFXInfo = ServiceLocator.Get<IPoolManager>().GetData<VFXInfo>();
            //
            await ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_Priest_UltimatePose, projectileTrans, projectileData, vFXInfo);
        }

        protected override IEnumerator OnUltimateCast(IBattleContext context)
        {
            // 获取施法者的动画组件
            var animationComponent = Caster.GetComponent<IBattleAnimationComponent>();
            // 设置技能对应的动画状态
            animationComponent.SetAnimationState(SkillInfo.f_animationType);
            
            // 等待动画切换到第一段
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(Priest_Ultimate_01));
            yield return UpdateCamera_01();
            
            // 等待动画切换到第二段
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(Priest_Ultimate_02));
            yield return UpdateCamera_02();

            CreateVFX();
            
            // 等待特效已结束，确保技能流程完成
            yield return new WaitUntil(() => !vFXInfo.IsAlive);
        }

        private IEnumerator UpdateCamera_01()
        {
            // 隐藏怪物UI
            ServiceLocator.Get<IUIManager>().GetController<BattleController>().MonsterStateUIManager.InActiveMonsterUIs();
            
            // 设置Mask，只看当前角色
            var mask = LayerGeter.GetPreBitLayer();
            mask |= 1 << Caster.GameObject.layer;
            
            // 切换相机视角看向角色
            var pos = Caster.GameObject.transform.position + Vector3.forward * 2.5f;
            pos = new Vector3(pos.x, 1, pos.z);
            var rot = Quaternion.Euler(0, 180, 0);
            yield return TaskUtility.WaitForTask(ServiceLocator.Get<IBattleCameraManager>().CreateCamera(null, pos, rot, mask));
        }
        
        private IEnumerator UpdateCamera_02()
        {
            // 显示怪物UI
            ServiceLocator.Get<IUIManager>().GetController<BattleController>().MonsterStateUIManager.ActiveMonsterUI(AllTargets.ToArray());
            
            // 设置Mask，看向怪物主目标、渲染所有目标
            var mask = LayerGeter.GetPreBitLayer();
            foreach (var battleEntityObject in AllTargets)
            {
                mask |= 1 << battleEntityObject.GameObject.layer;
            }
            
            var pos = new Vector3(MainTarget.GameObject.transform.position.x, 1, -2.5f);
            yield return TaskUtility.WaitForTask(ServiceLocator.Get<IBattleCameraManager>().
                CreateCamera(null, pos, Quaternion.identity, mask));
        }

        private async void CreateVFX()
        {
            // 重新初始化投射物数据（目标为主要攻击目标）
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            var pos = new Vector3(MainTarget.GameObject.transform.position.x, 5, 2.5f);
            // 更新投射物变换信息（基于主目标位置，无旋转）
            projectileTrans = new ProjectileTrans(pos, Quaternion.identity);
            vFXInfo = ServiceLocator.Get<IPoolManager>().GetData<VFXInfo>();
            // 创建终结技核心特效（命中目标处）
            await ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_Priest_UltiamteSkill, projectileTrans, projectileData, vFXInfo);
        }
    }
}
