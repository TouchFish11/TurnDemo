using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;

namespace GameHotUpdate.Battle.Event.General
{
    /// <summary>
    /// 护盾值变更事件类
    /// 用于在战斗中传递护盾值变化的相关信息
    /// </summary>
    public class ShieldChangedEvent : BattleEvent
    {
        /// <summary>
        /// 护盾变更的目标实体
        /// 表示哪个战斗实体的护盾发生了变化
        /// </summary>
        public IBattleEntityObject Target { get; }

        /// <summary>
        /// 当前护盾值
        /// 变更后目标实体的护盾最终数值
        /// </summary>
        public int CurrentShield { get; }

        /// <summary>
        /// 护盾变化量
        /// （新值 - 原始值）
        /// </summary>
        public int DeltaShield { get; }

        /// <summary>
        /// 护盾参考值
        /// 用于护盾相关计算的基准值，默认值为10000
        /// </summary>
        public int ReferenceShield => 10000;

        /// <summary>
        /// 护盾变更事件构造函数
        /// </summary>
        /// <param name="context">战斗上下文，包含战斗场景的核心信息</param>
        /// <param name="currentShield">变更后的当前护盾值</param>
        /// <param name="target">护盾发生变更的战斗实体</param>
        /// <param name="deltaShield">护盾变化量（增减数值）</param>
        public ShieldChangedEvent(IBattleContext context, int currentShield, IBattleEntityObject target, int deltaShield) : base(context)
        {
            CurrentShield = currentShield;
            Target = target;
            DeltaShield = deltaShield;
        }
    }
}