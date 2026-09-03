using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Event.Turn
{
    /// <summary>
    /// 额外回合重开选择事件（额外回合暂未实现，先占位保证编译）
    /// </summary>
    public class OperationReopenEvent : BattleEvent
    {
        public IBattleEntityObject CurrentBattleEntity { get; private set; }

        public OperationReopenEvent(IBattleContext context, IBattleEntityObject currentChar) : base(context)
        {
            CurrentBattleEntity = currentChar;
        }
    }
}