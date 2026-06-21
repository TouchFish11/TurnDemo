using System.Collections;
using HotUpdate.Base.Component;
using HotUpdate.Base.Enums;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Utility;

namespace HotUpdate.Game.Battle.Object.Role
{
    /// <summary>
    /// 角色死亡通用处理器
    /// </summary>
    public class RoleDeathHandler : DeathHandler
    {
        protected override IEnumerator OnHandle()
        {
            var animationComponent = battleEntityObject.GetComponent<IBattleAnimationComponent>();
             // 等待死亡动画播放结束
            yield return AnimationPlayUtility.WaitForAnimOver(animationComponent, AnimationUtility.Battle_Layer_Name, (int)E_AnimationType.Death);
        }
    }
}
