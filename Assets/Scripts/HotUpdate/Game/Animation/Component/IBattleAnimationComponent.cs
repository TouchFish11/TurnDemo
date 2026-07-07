namespace HotUpdate.Game.Animation.Component
{
    /// <summary>
    /// 战斗动画组件接口
    /// </summary>
    public interface IBattleAnimationComponent : IAnimationComponent
    {
        /// <summary>
        /// 设置技能动画状态
        /// </summary>
        /// <param name="stateName"></param>
        void SetSkillState(string stateName);
    }
}
