using System.Collections;
using Core.Utility;
using HotUpdate.Base.Component;
using HotUpdate.Base.UI;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.UI;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Strategys
{
    public class PriestUltimateFlowStrategy : UltimateFlowStrategy
    {
        private const string Priest_Ultimate_01 = nameof(Priest_Ultimate_01);
        private const string Priest_Ultimate_02 = nameof(Priest_Ultimate_02);

        protected override IEnumerator OnExecuteFlow()
        {
            // 获取施法者的动画组件
            var animationComponent = skillContext.Caster.GetComponent<IBattleAnimationComponent>();
            // 设置技能对应的动画状态
            animationComponent.SetAnimationState(skillContext.SkillInfo.f_animationType);
            
            // 等待动画切换到第一段
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(Priest_Ultimate_01));
            yield return UpdateCamera_01();
            
            // 等待动画切换到第二段
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(Priest_Ultimate_02));
            yield return UpdateCamera_02();

            CreateVFX();
            
            // 等待特效已结束，确保技能流程完成
            yield return new WaitUntil(() => !skillContext.VFXInfo.IsAlive);
        }

        private IEnumerator UpdateCamera_01()
        {
            // 隐藏怪物UI
            ((IBattleController)uiService.GetPanel(EUIPanelId.BattlePanel)).MonsterStateUIManager.InActiveMonsterUIs();
            
            // 设置Mask，只看当前角色
            var mask = LayerGeter.GetPreBitLayer();
            mask |= 1 << skillContext.Caster.GameObject.layer;
            
            // 切换相机视角看向角色
            var pos = skillContext.Caster.GameObject.transform.position + Vector3.forward * 2.5f;
            pos = new Vector3(pos.x, 1, pos.z);
            var rot = Quaternion.Euler(0, 180, 0);
            yield return TaskUtility.WaitForTask(battleCoordinator.SetCameraTrans(null, pos, rot, mask));
        }
        
        private IEnumerator UpdateCamera_02()
        {
            // 显示怪物UI
            (uiService.GetPanel(EUIPanelId.BattlePanel) as IBattleController).MonsterStateUIManager.ActiveMonsterUI(skillContext.AllTargets.ToArray());
            
            // 设置Mask，看向怪物主目标、渲染所有目标
            var mask = LayerGeter.GetPreBitLayer();
            foreach (var battleEntityObject in skillContext.AllTargets)
            {
                mask |= 1 << battleEntityObject.GameObject.layer;
            }
            
            var pos = new Vector3(skillContext.MainTarget.GameObject.transform.position.x, 1, -2.5f);
            yield return TaskUtility.WaitForTask(battleCoordinator.SetCameraTrans(null, pos, Quaternion.identity, mask));
        }

        private async void CreateVFX()
        {
            // 重新初始化投射物数据（目标为主要攻击目标）
            var projectileData = new ProjectileData(skillContext.Caster, skillContext.MainTarget, skillContext.AllTargets, this);
            skillContext.ProjectileData = projectileData;
            
            // 更新投射物变换信息（基于主目标位置，无旋转）
            var pos = new Vector3(skillContext.MainTarget.GameObject.transform.position.x, 5, 2.5f);
            var projectileTrans = new ProjectileTrans(pos, Quaternion.identity);
            skillContext.ProjectileTrans = projectileTrans;
            
            var vFXInfo = poolManager.GetData<VFXInfo>();
            skillContext.VFXInfo = vFXInfo;
            
            // 创建终结技核心特效（命中目标处）
            skillContext.Projectile = await vfxManager.CreateVFX(AssetKeys.VFX_Priest_UltiamteSkill, projectileTrans, projectileData, vFXInfo);
        }
    }
}
