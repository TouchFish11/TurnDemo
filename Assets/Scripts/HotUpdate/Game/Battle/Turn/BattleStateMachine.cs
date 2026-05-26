using System.Collections.Generic;
using Core.DI;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.StateMeachine;

namespace HotUpdate.Game.Battle.Turn
{
    /// <summary>
    /// 战斗状态机
    /// 控制战斗循环
    /// </summary>
    public class BattleStateMachine : IBattleStateMachine
    {
        // 战斗状态缓存
        private Dictionary<EBattlePhase, IBattleState> _battleStates = new();
        // 当前状态
        private IBattleState _currentState;
        
        public BattleStateMachine(IBattleContext context)
        {
            _battleStates.Add(EBattlePhase.Preparation, DIContainer.Create<PreparationState>(parameterValues: new object[] { this, context }));
            _battleStates.Add(EBattlePhase.EnterAnimation, DIContainer.Create<EnterAnimationState>(parameterValues: new object[] { this, context }));
            _battleStates.Add(EBattlePhase.TurnLoop, DIContainer.Create<TurnLoopState>(parameterValues: new object[] { this, context }));
            _battleStates.Add(EBattlePhase.Over, DIContainer.Create<BattleOverState>(parameterValues: new object[] { this, context }));
        }
        
        /// <summary>
        /// 开始战斗
        /// </summary>
        /// <returns></returns>
        public void StartBattle()
        {
            ChangeState(EBattlePhase.Preparation);
        }
        
        public void ChangeState(EBattlePhase battlePhase)
        {
            if (_currentState != null)
            {
                _battleStates[battlePhase].Exit();
            }
            
            _currentState = _battleStates[battlePhase];
            _currentState.Enter();
        }

        public void Dispose()
        {
            _currentState.Exit();
            _currentState = null;
            
            foreach (var state in _battleStates.Values)
            {
                state.Dispose();
            }
            _battleStates.Clear();
            _battleStates = null;
        }

        // /// <summary>
        // /// 开始战斗循环
        // /// </summary>
        // /// <returns></returns>
        // public IEnumerator StartBattle()
        // {
        //     yield return ActEntityTurn();
        //     BattleOver();
        // }

        // /// <summary>
        // /// 战斗准备
        // /// </summary>
        // public async Task BattlePreparation()
        // {
        //     //currentMonsterCount = _context.GetAliveMonsterEntitys().Count();
        //     // 创建战斗界面
        //     var battleController = await DIContainer.GetInstance<IUIManager>().CreateViewAsync<BattleView, BattleModel,BattleController>(E_UILayer.Mid, AssetKeys.BattleView);
        //     // 注册战斗界面UI调度器，依赖于战斗控制器
        //     DIContainer.GetInstance.Register<IBattleUIScheduler>(BattleUIScheduler.Instance); 
        //     // 显示战斗UI
        //     await battleController.InitBattleUI(_context);
        //     // 初始化行动顺序
        //     InitOrder();
        //     // 初始化行动实体
        //     UpdateActEntity();
        //     // 启用当前实体行动
        //     _currentActEntity.ExecuteAction();
        // }

        // /// <summary>
        // /// 实体行动回合
        // /// </summary>
        // private IEnumerator ActEntityTurn()
        // {
        //     while (true)
        //     {
        //         // 执行命令
        //         yield return commandsController.ExcuteCommand();
        //         // 检查战斗是否结束
        //         if (IsBattleOver)
        //         {
        //             yield break;
        //         }
        //
        //         // 当前实体正在行动，等待其行动结束
        //         if (_currentActEntity == null || !_currentActEntity.CanAct)
        //         {
        //             // 排序位置
        //             SortOrder();
        //             // 更新当前行动实体
        //             UpdateActEntity();
        //             // 启用当前实体行动
        //             _currentActEntity?.ExecuteAction();
        //         }
        //
        //         yield return null;
        //     }
        // }

        // /// <summary>
        // /// 更新当前行动实体
        // /// </summary>
        // public void UpdateActEntity()
        // {
        //     // 再让下一个实体行动
        //     _currentActEntity = _context.GetNextEntity();
        //     // 更新当前实体
        //     _context.SetCurrentEntity(_currentActEntity);
        //     // 更新实体看向
        //     UpdateEntityLookAt(_currentActEntity);
        // }

        // /// <summary>
        // /// 更新实体看向
        // /// </summary>
        // /// <param name="target"></param>
        // public void UpdateEntityLookAt(IBattleEntityObject target)
        // {
        //     if (target is PlayerObject)
        //     {
        //         //Transform playerTrans = BattlePoint.Instance.GetPlayerTransByIndex(target.EntityPosIndex);
        //         //Vector3 newPlayerPos = new Vector3(playerTrans.position.x, 0, playerTrans.position.z);
        //         //LogManager.Log($"玩家位置索引：{target.EntityPosIndex}；玩家位置；{newPlayerPos}");
        //
        //         // 所有怪物看向当前玩家
        //         //IEnumerable<Transform> monsterTrans = BattlePoint.Instance.GetMonsterTransforms();
        //         //foreach (var trans in monsterTrans)
        //         //{
        //         //    Vector3 newtransPos = new Vector3(trans.position.x, 0, trans.position.z);
        //         //    // 计算怪物在世界空间中需要的目标旋转（朝向玩家）
        //         //    //trans.rotation = Quaternion.LookRotation(newPlayerPos - newtransPos);
        //         //    Quaternion parentWorldRot = trans.parent.rotation;
        //         //    trans.localRotation = Quaternion.Inverse(parentWorldRot) * Quaternion.LookRotation(newPlayerPos - newtransPos);
        //         //    LogManager.Log($"怪物位置索引：{target.EntityPosIndex}；怪物位置；{newtransPos}");
        //         //}
        //     }
        //     else if (target is MonsterObject)
        //     {
        //         // 假设是单体攻击，怪物攻击哪个玩家，就激活哪个玩家的摄像机
        //     }
        // }

        // /// <summary>
        // /// 初始化顺序
        // /// 用于选取第一个行动的实体
        // /// </summary>
        // public void InitOrder()
        // {
        //     // 初始化所有角色的行动值
        //     foreach (var battleEntityObject in _context.GetAliveEntitys())
        //     {
        //         // 初始化行动值
        //         battleEntityObject.SetActionValue(CalcActionValue(battleEntityObject.GetSpeed()));
        //     }
        //
        //     // 基于行动值初始化行动顺序
        //     _context.Sort((b1, b2) =>
        //     {
        //         // 比较行动值确定行动顺序。行动值低，越先行动
        //         if (b1.ActionValue < b2.ActionValue)
        //         {
        //             return -1;
        //         }
        //
        //         if (b1.ActionValue > b2.ActionValue)
        //         {
        //             return 1;
        //         }
        //
        //         return 0;
        //     });
        //
        //     // TODO：暂时这样处理：第一个行动的实体行动值为0，后续可能根据算法优化
        //     _context.GetFirstBattleEntity().SetActionValue(0);
        //     // 事件分发传递，更新行动轴UI显示
        //     _context.GetEventBus().TriggerEvent(new ActionBarSortPostEvent(_context, _context.GetAliveEntitys()));
        // }

        // /// <summary>
        // /// 排序顺序
        // /// 模拟行动值的变化
        // /// </summary>
        // private void SortOrder()
        // {
        //     if (_currentActEntity == null)
        //     {
        //         return;
        //     }
        //
        //     // 暂时移除第一个角色，不参与计算
        //     _context.RemoveBattleEntity(_currentActEntity);
        //     var toatalSpeed = 0;
        //     // 重新计算剩下实体各自的剩余行动值
        //     foreach (var battleEntityObject in _context.GetAliveEntitys())
        //     {
        //         toatalSpeed += battleEntityObject.GetSpeed();
        //     }
        //
        //     foreach (var battleEntityObject in _context.GetAliveEntitys())
        //     {
        //         var oldAV = battleEntityObject.ActionValue;
        //         var newAV = (1 - battleEntityObject.GetSpeed() / (float)toatalSpeed) * oldAV;
        //         battleEntityObject.SetActionValue(newAV);
        //     }
        //
        //     // 基于行动值初始化行动顺序
        //     _context.Sort((c1, c2) =>
        //     {
        //         // 比较行动值确定行动顺序。行动值低，越先行动
        //         if (c1.ActionValue < c2.ActionValue)
        //         {
        //             return -1;
        //         }
        //         else if (c1.ActionValue > c2.ActionValue)
        //         {
        //             return 1;
        //         }
        //         else
        //         {
        //             return 0;
        //         }
        //     });
        //
        //     InsertOrder(_currentActEntity);
        //     _context.GetNextEntity().SetActionValue(0);
        //     // 事件分发传递，更新行动轴UI显示
        //     _context.GetEventBus().TriggerEvent(new ActionBarSortPostEvent(_context, _context.GetAliveEntitys()));
        // }

        // /// <summary>
        // /// 插入队列
        // /// </summary>
        // /// <param name="actEndEntity"></param>
        // public void InsertOrder(IBattleEntityObject actEndEntity)
        // {
        //     actEndEntity.SetActionValue(CalcActionValue(actEndEntity.GetSpeed()));
        //     var index = -1;
        //     foreach (var battleEntityObject in _context.GetAliveEntitys())
        //     {
        //         if (!(battleEntityObject.ActionValue > actEndEntity.ActionValue))
        //         {
        //             continue;
        //         }
        //         
        //         index = _context.GetEntityIndex(battleEntityObject);
        //         // 找到第一个行动值大于当前角色的索引，插入到该位置前
        //         _context.Insert(index, actEndEntity);
        //         break;
        //     }
        //
        //     if (index == -1)
        //     {
        //         // 所有角色行动值都更小，插入末尾
        //         _context.AddBattleEntity(actEndEntity);
        //     }
        // }

        // /// <summary>
        // /// 计算行动值
        // /// </summary>
        // /// <param name="speed"></param>
        // /// <returns></returns>
        // private static float CalcActionValue(float speed)
        // {
        //     // 计算行动值，基准行动值 / 速度 * 修正系数
        //     return BASE_ACTION_VALUE / speed * SPEED_CORRECTION;
        // }

        // /// <summary>
        // /// 检查战斗是否结束
        // /// </summary>
        // /// <returns></returns>
        // public bool CheckBattleOver()
        // {
        //     foreach (var battleOverCondition in battleOverConditions)
        //     {
        //         // 每次执行完命令后，检查战斗是否结束
        //         IsBattleOver = battleOverCondition.CheckOver(_context);
        //         if (IsBattleOver)
        //         {
        //             return true;
        //         }
        //     }
        //     
        //     return false;
        // }

        // /// <summary>
        // /// 移除死亡怪物实体
        // /// </summary>
        // public IEnumerator RemoveDeadMonster()
        // {
        //     var  deadEntities = new List<IBattleEntityObject>();
        //     foreach (var battleEntity in _context.GetDeadEntitys())
        //     {
        //         yield return battleEntity.Die();
        //         deadEntities.Add(battleEntity);
        //
        //         if (battleEntity == _currentActEntity)
        //         {
        //             _currentActEntity = null;
        //         }
        //         if (battleEntity is MonsterObject)
        //         {
        //             UnityEngine.Object.Destroy(battleEntity.GameObject);
        //         }
        //     }
        //     
        //     // 移除
        //     foreach (var battleEntityObject in deadEntities)
        //     {
        //         _context.RemoveBattleEntity(battleEntityObject);
        //
        //         if (battleEntityObject is PlayerObject)
        //         {
        //             _context.RemoveSceneRole(battleEntityObject);
        //         }
        //         else
        //         {
        //             _context.RemoveSceneMonster(battleEntityObject);
        //         }
        //     }
        //     
        //     // 事件分发传递，更新行动轴UI显示
        //     _context.GetEventBus().TriggerEvent(new ActionBarSortPostEvent(_context, _context.GetAliveEntitys()));
        // }

        // /// <summary>
        // /// 插入命令
        // /// </summary>
        // /// <param name="command"></param>
        // public void InsertCommand(ICommand command)
        // {
        //     commandsController.InsertCommand(command);
        // }

        // /// <summary>
        // /// 战斗结束
        // /// </summary>
        // private void BattleOver()
        // {
        //     // 切换为正常倍速
        //     DIContainer.GetInstance<ITimerManager>().SetTimeRate(E_TimeRate.Normal);
        //     // 触发战斗结束事件
        //     _context.GetEventBus().TriggerEvent(new BattleOverEvent(_context));
        // }
    }
}
