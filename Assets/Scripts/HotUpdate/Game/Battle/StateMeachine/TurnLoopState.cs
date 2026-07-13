using System.Collections;
using Core.DI;
using Core.Mono;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Turn;
using HotUpdate.Game.Battle.Utility;

namespace HotUpdate.Game.Battle.StateMeachine
{
    /// <summary>
    /// 回合循环状态
    /// </summary>
    public class TurnLoopState : BattleState
    {
        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private IBattleManager _battleManager;
        [Inject] private IMonsterFactory _monsterFactory;
        [Inject] private IBattleCommandsController _commandsController;

        private bool _isOver;
        
        public TurnLoopState(IBattleStateMachine battleStateMachine, IBattleContext context) : base(battleStateMachine, context)
        {
            
        }

        public override void Enter()
        {
            // 监听插入指令事件
            Context.EventBus.AddListener<InsertCommandEvent>(OnInsertCommand);
            // 开启战斗回合协程
            _monoAdapter.StartCoroutine(TurnLoop_Cor());
        }
        
        /// <summary>
        /// 回合循环
        /// </summary>
        private IEnumerator TurnLoop_Cor()
        {
            while (true)
            {
                while (Context.CurrentCommand != null || Context.BattleCommands.Count > 0)
                {
                    // 执行指令
                    yield return _commandsController.ExcuteCommand();
                    
                    var battleService = _battleManager.BattleService;
                    // 处理存在的死亡的实体
                    yield return battleService.HandleDeadEntity();
                    // 过滤无效指令
                    _commandsController.FilterInvalidCommand();
                    // 执行完一次指令都要检查当前波次战斗是否结束
                    if (battleService.CheckWaveOver())
                    {
                        if (battleService.HasRemainWave())
                        {
                            // 切换到下一波
                            yield return battleService.MoveWave();
                        }
                        else
                        {
                            _isOver = true;
                            break;
                        }
                    }
                    else
                    {
                        // 指令执行后处理逻辑
                        yield return _commandsController.ExcutePostProcess();
                    }
                }
                
                if(_isOver)
                    break;
                
                // 当前实体正在行动，等待其行动结束
                if (Context.CurrentTurnOwner == null || !Context.CurrentTurnOwner.CanAct && !Context.CurrentTurnOwner.Acting)
                {
                    BattleUtility.UpdateOrder(Context);
                    // 更新当前行动实体
                    Context.SetCurrentTurnOwner(Context.AllBattleEntity[0]);
                    // 启用当前实体行动
                    Context.CurrentTurnOwner.ExecuteAction();
                }

                yield return null;
            }
            
            BattleStateMachine.ChangeState(EBattlePhase.Over);
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
            _isOver = false;
        }

        protected override void OnDispose()
        {
            Context.EventBus.RemoveListener<InsertCommandEvent>(OnInsertCommand);
            _commandsController = null;
            _battleManager = null;
            _monoAdapter = null;
        }
    }
}
