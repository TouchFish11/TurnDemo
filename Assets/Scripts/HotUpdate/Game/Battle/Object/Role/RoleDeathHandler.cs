using System.Collections;
using HotUpdate.Base.Animation;
using HotUpdate.Game.Animation.Component;
using AnimationUtility = HotUpdate.Game.Battle.Utility.AnimationUtility;

namespace HotUpdate.Game.Battle.Object.Role
{
    /// <summary>
    /// 角色死亡通用处理器
    /// </summary>
    public class RoleDeathHandler : DeathHandler
    {
        protected override IEnumerator OnHandle()
        {
            var animationComponent = battleEntityObject.GetComponent<BattleAnimationComponent>();
             // 等待死亡动画播放结束
            yield return AnimationUtility.WaitForCommonAnimOver(animationComponent, Base.Utility.AnimationLayer.Battle_Layer_Name, EAnimationType.Death);
            // 角色留在场上
        }
    }
}
