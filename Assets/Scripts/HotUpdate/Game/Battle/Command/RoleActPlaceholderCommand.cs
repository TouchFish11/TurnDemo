using System.Collections;
using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Command
{
    /// <summary>
    /// 玩家角色行动占位指令，没有实际效果，仅在等待UI中起到占位作用
    /// </summary>
    public class RoleActPlaceholderCommand : Command
    {
        public override int Priority { get; protected set; } = 2;
        
        public override IEnumerator Execute(IBattleContext context)
        {
            // var playerObject = (PlayerObject)Sender;
            // playerObject.ChangeState(playerObject.CurrentActPhase);
            yield break;
        }

        public override IEnumerator ExcutePostProcess(IBattleContext context)
        {
            yield break;
        }
    }
}
