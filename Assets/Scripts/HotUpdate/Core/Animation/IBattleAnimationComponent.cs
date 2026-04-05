namespace HotUpdate.Core.Animation
{
    public interface IBattleAnimationComponent : IAnimationComponent
    {
        /// <summary>
        /// 设置必杀技姿态（触发预必杀技攻击动画）
        /// 提供给外部调用的快捷方法
        /// </summary>
        void SetUltimatePose();
    }
}
