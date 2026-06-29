using System.Collections;
using Core.Utility;
using HotUpdate.Base.Enums;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Utility;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster
{
    /// <summary>
    /// 怪物死亡通用处理器
    /// </summary>
    public class MonsterDeathHandler : DeathHandler
    {
        protected override IEnumerator OnHandle()
        {
            // 初始化特效信息对象（用于追踪特效生命周期）
            var vFXInfo = poolManager.GetData<VFXInfo>();
            
            // 创建并播放怪物死亡特效
            // 参数说明：特效资源Key → 特效挂载节点 → 投射数据（关联当前怪物）→ 特效信息（用于后续判断）
            yield return TaskUtility.WaitForTask(vfxManager.CreateVFX(
                AssetKeys.VFX_MonsterDead,
                new ProjectileTrans(battleEntityObject.GameObject.transform, false),
                new ProjectileData(battleEntityObject, null, null, null),
                vFXInfo));

            var animationComponent =  battleEntityObject.GetComponent<BattleAnimationComponent>();
            // 播放怪物死亡动画，并等待动画播放完成
            animationComponent.StartCoroutine(AnimationPlayUtility.WaitForAnimOver(animationComponent, AnimationUtility.Battle_Layer_Name, (int)E_AnimationType.Death));
            // 等待死亡特效播放完毕（协程阻塞，直到特效销毁）
            yield return new WaitUntil(() => !vFXInfo.IsAlive);
        }
    }
}
