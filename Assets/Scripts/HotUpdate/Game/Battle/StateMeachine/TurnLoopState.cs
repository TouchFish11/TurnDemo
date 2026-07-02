using System.Collections;
using System.Collections.Generic;
using Core.DI;
using Core.Mono;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Object.Role;
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
        
        // 战斗指令控制器
        private BattleCommandsController _commandsController;
        // 战斗是否结束
        private bool _isBattleOver;
        
        public TurnLoopState(IBattleStateMachine battleStateMachine, IBattleContext context) : base(battleStateMachine, context)
        {
            // 创建战斗指令控制器实例
            _commandsController = DIContainer.Create<BattleCommandsController>(parameterValues: new object[] { this, Context });
        }

        public override void Enter()
        {
            // 监听插入指令事件
            Context.GetEventBus().AddListener<InsertCommandEvent>(OnInsertCommand);
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
                // 执行指令
                yield return _commandsController.ExcuteCommand();
                
                // 执行完一次指令都要检查战斗是否结束
                if (_isBattleOver)
                {
                    break;
                }

                // 当前实体正在行动，等待其行动结束
                if (Context.CurrentTurnOwner == null || !Context.CurrentTurnOwner.CanAct)
                {
                    BattleUtility.UpdateOrder(Context);
                    // 更新当前行动实体
                    Context.SetCurrentTurnOwner(Context.GetNextEntity());
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
        
        /// <summary>
        /// 移除死亡怪物实体
        /// </summary>
        public IEnumerator RemoveDeadMonster()
        {
            var  deadEntities = new List<IBattleEntityObject>();
            foreach (var battleEntity in Context.GetDeadEntitys())
            {
                yield return battleEntity.Die();
                deadEntities.Add(battleEntity);

                if (battleEntity == Context.CurrentTurnOwner)
                {
                    Context.SetCurrentTurnOwner(null);
                }
                if (battleEntity is MonsterObject)
                {
                    battleEntity.Destroy();
                    EngineUtility.Destroy(battleEntity.GameObject);
                }
            }
            
            // 移除
            foreach (var battleEntityObject in deadEntities)
            {
                Context.RemoveBattleEntity(battleEntityObject);

                if (battleEntityObject is PlayerObject)
                {
                    Context.RemoveSceneRole(battleEntityObject);
                }
                else
                {
                    Context.RemoveSceneMonster(battleEntityObject);
                }
            }
        }
        
        /// <summary>
        /// 检查当前波次是否结束
        /// </summary>
        /// <returns></returns>
        public bool CheckWaveOver()
        {
            // 每次执行完命令后，检查战斗是否结束
            _isBattleOver = _battleManager.GetWaveCreator().CheckOver();
            if (_isBattleOver)
            {
                return true;
            }
            return false;
        }

        public void MoveWave()
        {
            if (_battleManager.GetWaveCreator().TryMoveWave())
            {
                _monoAdapter.StartCoroutine(_battleManager.GetBattleService().UpdateWave());
            }
        }

        public override void Exit()
        {

        }

        protected override void OnDispose()
        {
            Context.GetEventBus().RemoveListener<InsertCommandEvent>(OnInsertCommand);
            _commandsController = null;
            _battleManager = null;
            _monoAdapter = null;
        }
    }
}
