using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Event.UI
{
    /// <summary>
    /// 行动格子点击事件，显示对应的角色当前的所有Buff状态
    /// </summary>
    public class ActionGridClickEvent : BattleEvent
    {
        public IBattleEntityObject BattleEntity { get; private set; }
        
        public ActionGridClickEvent(IBattleContext context, IBattleEntityObject battleEntity) : base(context)
        {
            BattleEntity = battleEntity;
        }
    }
}
