
namespace Game.Battle
{
    /// <summary>
    /// 血量变化事件
    /// </summary>
    public class OnHpChangedEvent : BattleEvent
    {
        public int CurrentHp { get; private set; }
        public int MaxHp { get; private set; }

        public OnHpChangedEvent(IBattleContext context, int currentHp, int maxHp) : base(context)
        {
            CurrentHp = currentHp;
            MaxHp = maxHp;
        }
    }
}
