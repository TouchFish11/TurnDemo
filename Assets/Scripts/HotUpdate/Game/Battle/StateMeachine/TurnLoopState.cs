using System.Collections;
using System.Collections.Generic;
using Core.DI;
using Core.Mono;
using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Condition;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Base.Battle.Property;
using HotUpdate.Base.Battle.Turn;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Condition;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Property;
using HotUpdate.Game.Battle.Utility;

namespace HotUpdate.Game.Battle.StateMeachine
{
    /// <summary>
    /// 回合循环状态
    /// </summary>
    public class TurnLoopState : BattleState
    {
        // 战斗指令控制器
        private BattleCommandsController _commandsController;
        // 战斗是否结束
        private bool _isBattleOver;
        // 当前行动实体
        private IBattleEntityObject _currentActEntity;
        // 当前战斗结束条件
        private List<IBattleOverCondition> battleOverConditions = new();
        
        public TurnLoopState(IBattleStateMachine battleStateMachine, IBattleContext context) : base(battleStateMachine, context)
        {

        }

        public override void Enter()
        {
            // 创建战斗指令控制器实例
            _commandsController = new BattleCommandsController(this);
            // 监听插入指令
            Context.GetEventBus().AddListener<InsertCommandEvent>(OnInsertCommand);
            // TODO：写死，后续根据配置优化
            battleOverConditions.Add(new AllTurnOverCondition());
            battleOverConditions.Add(new AllPlayerDeadCondition());
            
            Execute();
        }

        public override void Execute()
        {
            DIContainer.GetInstance<IMonoAdapter>().StartCoroutine(TurnLoop_Cor());
        }
        
        /// <summary>
        /// 回合循环
        /// </summary>
        private IEnumerator TurnLoop_Cor()
        {
            while (true)
            {
                // 执行命令
                yield return _commandsController.ExcuteCommand();
                // 检查战斗是否结束
                if (_isBattleOver)
                {
                    break;
                }

                // 当前实体正在行动，等待其行动结束
                if (_currentActEntity == null || !_currentActEntity.CanAct)
                {
                    // 排序位置
                    SortOrder();
                    // 更新当前行动实体
                    UpdateActEntity();
                    // 启用当前实体行动
                    _currentActEntity?.ExecuteAction();
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

                if (battleEntity == _currentActEntity)
                {
                    _currentActEntity = null;
                }
                if (battleEntity is MonsterObject)
                {
                    battleEntity.Destroy();
                    UnityEngine.Object.Destroy(battleEntity.GameObject);
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
            
            // 事件分发传递，更新行动轴UI显示
            Context.GetEventBus().TriggerEvent(new ActionBarSortPostEvent(Context, Context.GetAliveEntitys()));
        }
        
        /// <summary>
        /// 排序顺序
        /// 模拟行动值的变化
        /// </summary>
        private void SortOrder()
        {
            if (_currentActEntity == null)
            {
                return;
            }

            // 暂时移除第一个角色，不参与计算
            Context.RemoveBattleEntity(_currentActEntity);
            var toatalSpeed = 0;
            // 重新计算剩下实体各自的剩余行动值
            foreach (var battleEntityObject in Context.GetAliveEntitys())
            {
                var speed = battleEntityObject.GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.CurrentSpeed);
                toatalSpeed += speed;
            }

            foreach (var battleEntityObject in Context.GetAliveEntitys())
            {
                var oldAV = battleEntityObject.ActionValue;
                var speed = battleEntityObject.GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.CurrentSpeed);
                var newAV = (1 - speed / (float)toatalSpeed) * oldAV;
                battleEntityObject.SetActionValue(newAV);
            }

            // 基于行动值初始化行动顺序
            Context.Sort((c1, c2) =>
            {
                // 比较行动值确定行动顺序。行动值低，越先行动
                if (c1.ActionValue < c2.ActionValue)
                {
                    return -1;
                }

                return c1.ActionValue > c2.ActionValue ? 1 : 0;
            });

            InsertOrder(_currentActEntity);
            Context.GetNextEntity().SetActionValue(0);
            // 事件分发传递，更新行动轴UI显示
            Context.GetEventBus().TriggerEvent(new ActionBarSortPostEvent(Context, Context.GetAliveEntitys()));
        }
        
        /// <summary>
        /// 插入队列
        /// </summary>
        /// <param name="actEndEntity"></param>
        public void InsertOrder(IBattleEntityObject actEndEntity)
        {
            var speed = actEndEntity.GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.CurrentSpeed);
            actEndEntity.SetActionValue(BattleUtility.CalcActionValue(speed));
            var index = -1;
            foreach (var battleEntityObject in Context.GetAliveEntitys())
            {
                if (!(battleEntityObject.ActionValue > actEndEntity.ActionValue))
                {
                    continue;
                }
                
                index = Context.GetEntityIndex(battleEntityObject);
                // 找到第一个行动值大于当前角色的索引，插入到该位置前
                Context.Insert(index, actEndEntity);
                break;
            }

            if (index == -1)
            {
                // 所有角色行动值都更小，插入末尾
                Context.AddBattleEntity(actEndEntity);
            }
        }
        
        /// <summary>
        /// 更新当前行动实体
        /// </summary>
        public void UpdateActEntity()
        {
            // 再让下一个实体行动
            _currentActEntity = Context.GetNextEntity();
            // 更新当前实体
            Context.SetCurrentEntity(_currentActEntity);
        }
        
        /// <summary>
        /// 检查战斗是否结束
        /// </summary>
        /// <returns></returns>
        public bool CheckBattleOver()
        {
            foreach (var battleOverCondition in battleOverConditions)
            {
                // 每次执行完命令后，检查战斗是否结束
                _isBattleOver = battleOverCondition.CheckOver(Context);
                if (_isBattleOver)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Exit()
        {
            // TODO：结束当前回合的循环
            
        }

        public override void Dispose()
        {
            Context.GetEventBus().RemoveListener<InsertCommandEvent>(OnInsertCommand);
            battleOverConditions.Clear();
            battleOverConditions = null;
            _commandsController = null;
            
            base.Dispose();
        }
    }
}
