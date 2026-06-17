using System.Collections;
using Core.DI;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Status;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 处理弹射物命中触发事件节点
    /// </summary>
    public class ProcessProjectileEventNode : SkillNode
    {
        // 状态工厂
        [Inject] private IStatusFactory _statusFactory;
        // 伤害计算管理器
        [Inject] private IDamageCalcManager _damageCalcManager;
        // 特效管理器
        [Inject] private IVFXManager _vfxManager;
        
        public ProcessProjectileEventNode(ISkill skill) : base(skill)
        {
            
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override IEnumerator Execute()
        {
            SkillContext.Projectile.OnTrigger += OnTrigger;
            yield break;
        }

        private async void OnTrigger(HitResult hitResult)
        {
            var projectileData = SkillContext.ProjectileData;
            
            // 先添加且只添加一次buff
            if (hitResult.IsFirstHit)
            {
                // 添加buff
                foreach (var target in projectileData.targets)
                {
                    foreach (var statusId in SkillContext.StatusIds)
                    {
                        // 获取状态实例
                        var status = _statusFactory.GetStatus(projectileData.caster, target, statusId);
                        // 添加状态
                        target.GetComponent<StatusComponent>().AddStatus(status);
                    }
                }

            }
            
            // 这里可以直接移除特效
            SkillContext.VFXInfo.IsStop = true;

            // 每段的伤害计算
            foreach (var target in projectileData.targets)
            {
                _damageCalcManager.CalcSkillDamage(projectileData.caster, target, SkillContext.SkillInfo, out var result);
                target.TakeDamage(result);
            }
            
            // 每段的命中特效创建
            foreach (var target in projectileData.targets)
            {
                var projectileTrans = new ProjectileTrans(target.GameObject.transform.position + Vector3.up * 0.5f, Quaternion.identity);
                var vfxInfo = new VFXInfo();
                await _vfxManager.CreateVFX(AssetKeys.VFX_MonsterHit, projectileTrans, default, vfxInfo);
            }
        }
    }
}
