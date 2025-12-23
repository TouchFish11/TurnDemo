
namespace Game.Battle
{
    /// <summary>
    /// 血量变化事件
    /// </summary>
    public class OnHpChangedEvent : BattleEvent
    {
        public IBattleEntityObject Target { get; private set; }
        public int CurrentHp { get; private set; }
        public int MaxHp { get; private set; }

        /// <summary>
        /// 血量差值（原数值 - 现数值）
        /// 正数为伤害/最大生命降低，负数为治疗/最大生命提高
        /// </summary>
        public int DeltaHp { get; private set; }

        public OnHpChangedEvent(IBattleContext context, int currentHp, int maxHp, int deltaHp, IBattleEntityObject target) : base(context)
        {
            CurrentHp = currentHp;
            MaxHp = maxHp;
            DeltaHp = deltaHp;
            Target = target;
        }
    }
}
