using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;

namespace GameHotUpdate.Battle.Event.General
{
    /// <summary>
    /// 生命值变更事件类
    /// 用于在战斗流程中传递实体对象的生命值变化信息
    /// </summary>
    public class HpChangedEvent : BattleEvent
    {
        /// <summary>
        /// 生命值变更的目标实体
        /// （如玩家、怪物等实现IBattleEntityObject接口的战斗对象）
        /// </summary>
        public IBattleEntityObject Target { get; private set; }

        /// <summary>
        /// 变更后当前的生命值
        /// 取值范围：0 ≤ CurrentHp ≤ MaxHp
        /// </summary>
        public int CurrentHp { get; private set; }

        /// <summary>
        /// 目标实体的最大生命值
        /// </summary>
        public int MaxHp { get; private set; }

        /// <summary>
        /// 生命值变更事件的构造函数
        /// </summary>
        /// <param name="context">战斗上下文（包含当前战斗的核心环境信息）</param>
        /// <param name="currentHp">变更后的当前生命值</param>
        /// <param name="maxHp">目标实体的最大生命值</param>
        /// <param name="target">生命值变更的目标实体</param>
        public HpChangedEvent(IBattleContext context, int currentHp, int maxHp, IBattleEntityObject target) : base(context)
        {
            CurrentHp = currentHp;
            MaxHp = maxHp;
            Target = target;
        }
    }
}