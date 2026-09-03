using System.Collections;
using Core.DI;
using Core.Tasks;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Command
{
    /// <summary>
    /// 战斗实体回合开始命令，用于结算buff和Dot
    /// </summary>
    public class TurnStartCommand : Command
    {
        [Inject] private IBattleManager _battleManager;
        
        public override IBattleEntityObject Sender { get; protected set; }
        
        public override ECommandPriority Priority { get; protected set;} = ECommandPriority.TurnStart;

        public void Init(IBattleEntityObject sender)
        {
            Sender = sender;
        }
        
        public override IEnumerator Execute(IBattleContext context)
        {
            Sender.GetComponent<StatusComponent>().SettlementTurnStart();
            yield return _battleManager.BattleService.PlaySettlementDot(Sender).ToCoroutine();
        }

        public override IEnumerator ExcutePostProcess(IBattleContext context)
        {
            if (Sender.CanAct && !Sender.IsDead)
            {
                context.EventBus.TriggerEvent(new TurnStartEvent(context, Sender));
                // 打开操作：怪物=自动释放技能；角色=等待输入
                Sender.TurnActionDriver.OnOperationOpened();
            }
            yield break;
        }
    }
}
