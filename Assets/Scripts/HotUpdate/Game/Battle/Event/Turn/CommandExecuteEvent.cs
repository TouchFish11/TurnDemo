using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Event.Turn
{
    /// <summary>
    /// 指令执行事件，更新当前执行指令的对象图标UI显示；
    /// </summary>
    public class CommandExecuteEvent : BattleEvent
    {
        public ICommand CurrentCommand { get; }
        
        public CommandExecuteEvent(IBattleContext context, ICommand currentCommand) : base(context)
        {
            CurrentCommand = currentCommand;
        }
    }
}
