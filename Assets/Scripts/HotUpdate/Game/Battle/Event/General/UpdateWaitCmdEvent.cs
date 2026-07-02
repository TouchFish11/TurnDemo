using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Event.General
{
    /// <summary>
    /// 更新指令等待事件
    /// </summary>
    public class UpdateWaitCmdEvent : BattleEvent
    {
        public IBattleEntityObject CurrentEntity { get; }
        
        public bool AddOrRemoveCmd { get; }
        
        public int Priority { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="battleEntity"></param>
        /// <param name="priority"></param>
        /// <param name="isAdd">true为新增等待指令；false为移除等待指令</param>
        public UpdateWaitCmdEvent(IBattleContext context, IBattleEntityObject battleEntity, int priority, bool isAdd) : base(context)
        {
            CurrentEntity = battleEntity;
            AddOrRemoveCmd = isAdd;
            Priority = priority;
        }
    }
}
