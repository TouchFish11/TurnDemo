namespace HotUpdate.Game.Battle.Skill.Base
{
    /// <summary>
    /// 技能命中结果
    /// </summary>
    public struct HitResult
    {
        /// <summary>
        /// 是否是首次命中
        /// </summary>
        public bool IsFirstHit { get; private set; }
        
        /// <summary>
        /// 当前的伤害段数索引
        /// </summary>
        public int CurrentHitIndex { get; private set; }
        
        public HitResult(bool isFirstHit, int currentHitIndex)
        {
            IsFirstHit = isFirstHit;
            CurrentHitIndex = currentHitIndex;
        }
    }
}
