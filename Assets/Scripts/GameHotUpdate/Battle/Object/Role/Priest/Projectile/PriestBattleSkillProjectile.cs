using Core.Reflection;
using Core.Service;
using Game.Battle.Enum;
using Game.Battle.Status;
using Game.VFX;
using GameHotUpdate.Battle.Projectile;
using GameHotUpdate.Battle.Property;
using GameHotUpdate.Battle.Status;
using UnityEngine;

namespace GameHotUpdate.Battle.Object.Role.Priest.Projectile
{
    /// <summary>
    /// 牧师战技弹射物
    /// </summary>
    public class PriestBattleSkillProjectile : InstantProjectile
    {
        protected override void AddStatusOnTrigger()
        {
            foreach (var target in projectileData.targets)
            {
                foreach (var statusId in statusIds)
                {
                    // 获取状态实例
                    var status = ServiceLocator.Get<IFactoryManager>().GetFactory<IStatusFactory, StatusFactory>().
                        GetStatus(projectileData.caster, target, statusId);
                    // 添加状态
                    target.GetComponent<StatusComponent>().AddStatus(status);
                } 
            }
        }
        
        protected override void CreateVFXOnTrigger()
        {
            foreach (var target in projectileData.targets)
            {
                var projectileTrans = new ProjectileTrans(target.GameObject.transform.position, Quaternion.identity);
                vFXInfo = new VFXInfo();
                //ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_BlueHit, projectileTrans, default, vFXInfo);
            }
        }
        
        protected override void CauseDamageOnTrigger()
        {
            // 回血
            foreach (var target in projectileData.targets)
            {
                var newCurrentHp = target.GetComponent<PropertyComponent>()
                    .GetPropertyValue(E_DynamicPropertyType.CurrentHp) + 100;
                target.GetComponent<PropertyComponent>().SetPropertyValue(E_DynamicPropertyType.CurrentHp, newCurrentHp);
            }
        }

        protected override void HandleOtherOnTrigger()
        {
            
        }
    }
}
