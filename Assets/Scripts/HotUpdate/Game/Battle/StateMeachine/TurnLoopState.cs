using System.Collections;
using Core.DI;
using Core.Mono;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Statuses;
using HotUpdate.Game.Battle.Turn;
using HotUpdate.Game.Battle.Utility;

namespace HotUpdate.Game.Battle.StateMeachine
{
    public class TurnLoopState : BattleState
    {
        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private IBattleManager _battleManager;
        [Inject] private IMonsterFactory _monsterFactory;
        [Inject] private IBattleCommandsController _commandsController;
        
        private BattleService BattleService => _battleManager.BattleService;
        
        public TurnLoopState(IBattleStateMachine battleStateMachine, IBattleContext context) : base(battleStateMachine, context)
        {
        
        }

        public override void Enter()
        {
            // 监听插入指令事件
            Context.EventBus.AddListener<InsertCommandEvent>(OnInsertCommand);
            // 开启战斗回合协程
            _monoAdapter.StartCoroutine(TurnLoop());
        }
        
        /// <summary>
        /// 回合循环
        /// </summary>
        private IEnumerator TurnLoop()
        {
            while (!Context.IsOver)
            {
                // 有命令在执行（槽位被 CurrentCommand 占着）
                if (Context.CurrentCommand != null || Context.BattleCommands.Count > 0)
                {
                    // 执行指令
                    yield return _commandsController.ExcuteCommand();
                    // 处理存在的死亡的实体
                    yield return BattleService.HandleDeadEntity();
                    // 过滤无效指令
                    _commandsController.FilterInvalidCommand();
                    // 执行完一次指令都要检查当前波次战斗是否结束
                    if (BattleService.CheckWaveOver())
                    {
                        if (BattleService.HasRemainWave())
                        {
                            // 转波次：清空当前指令 + 重置回合持有者，不执行 PostProcess
                            _commandsController.ClearCurrentCommand();
                            Context.SetCurrentTurnOwner(null);
                            // 切换到下一波
                            yield return BattleService.MoveWave();   
                            continue;   // 跳过 PostProcess
                        }
                        
                        Context.IsOver = true;     // 没有剩余波次 → 战斗结束
                        break;
                    }
                    
                    yield return _commandsController.ExcutePostProcess();
                    continue;
                }

                var owner = Context.CurrentTurnOwner;
                // 队列空 且 已交回合 → 结束回合、推进
                if (owner == null || owner.TurnFinished || owner.IsDead)
                {
                    if (owner != null && !owner.IsDead) 
                        yield return FinishTurn(owner);
                    AdvanceToNext();
                    continue;
                }
                
                // 队列空 且 未交回合 → 正常待操作（槽位空，终结技插进来立即执行）
                yield return null;
            }
            BattleStateMachine.ChangeState(EBattlePhase.Over);
        }
        
        private IEnumerator FinishTurn(IBattleEntityObject owner)
        {
            owner.GetComponent<StatusComponent>().SettlementTurnEnd();
            Context.EventBus.TriggerEvent(new TurnEndEvent(Context, owner));
            yield break;
        }
        
        private void AdvanceToNext()
        {
            BattleUtility.UpdateOrder(Context);
            if (Context.RemainRound <= 0)
            {
                Context.IsOver = true; 
                return;
            }
            var next = Context.AllBattleEntity[0];
            Context.SetCurrentTurnOwner(next);
            next.ExecuteAction();
        }

        /// <summary>
        /// 插入命令
        /// </summary>
        /// <param name="commandEvent"></param>
        private void OnInsertCommand(InsertCommandEvent commandEvent)
        {
            _commandsController.InsertCommand(commandEvent.Command);
        }
        
        public override void Exit()
        {

        }

        protected override void OnDispose()
        {

        }
    }
}
