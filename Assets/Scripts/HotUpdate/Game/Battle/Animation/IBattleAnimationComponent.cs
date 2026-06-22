using HotUpdate.Base.Component;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Animation
{
    public interface IBattleAnimationComponent : IAnimationComponent, IBattleComponent
    {
        /// <summary>
        /// 初始化战斗动画组件
        /// </summary>
        /// <param name="battleEntity"></param>
        void InitBattleAnimation(IBattleEntityObject battleEntity);
        
        /// <summary>
        /// 设置必杀技姿态（触发预必杀技攻击动画）
        /// 提供给外部调用的快捷方法
        /// </summary>
        void SetUltimatePose();
    }
}
