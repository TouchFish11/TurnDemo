using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 回合控制器
    /// 控制战斗循环
    /// </summary>
    public class TurnController
    {
        // 战斗上下文
        private readonly IBattleContext _context;
        // 战斗指令控制器
        private readonly BattleCommandsController commandsController;
        // 当前行动实体
        private IBattleEntityObject _currentActEntity;
        // 当前战斗结束条件
        private IBattleOverCondition battleOverCondition;
        // 当前怪物数量
        private int currentMonsterCount;

        /// <summary>
        /// 战斗是否结束
        /// </summary>
        public bool IsBattleOver { get; private set; }

        /// <summary>
        /// 基础行动值
        /// </summary>
        private const float BASE_ACTION_VALUE = 10000f;

        /// <summary>
        /// 速度修正系数（平衡不同速度区间）
        /// </summary>
        private const float SPEED_CORRECTION = 1.0f;

        public TurnController(IBattleContext context, IBattleOverCondition battleOverCondition)
        {
            _context = context;
            commandsController = new BattleCommandsController(context);
            this.battleOverCondition = battleOverCondition;
        }

        /// <summary>
        /// 战斗循环
        /// </summary>
        /// <returns></returns>
        public IEnumerator BattleLoop()
        {
            yield return TaskUtility.WaitForTask(BattlePreparation());
            yield return ActEntityTurn();
            BattleOver();
        }

        /// <summary>
        /// 战斗准备
        /// </summary>
        private async Task BattlePreparation()
        {
            currentMonsterCount = _context.GetMonsterObjects().Count;
            // 创建战斗界面
            BattleController battleController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<BattleView, BattleModel,BattleController>(E_UILayer.Mid);
            // 播放入场动画等
            // ...
            // 显示战斗UI
            await battleController.InitBattleUI(_context);
            // 初始化行动顺序
            InitOrder();
            // 初始化行动实体
            UpdateActEntity();
            // 启用当前实体行动
            _currentActEntity.ExecuteAction();
        }

        /// <summary>
        /// 实体行动回合
        /// </summary>
        private IEnumerator ActEntityTurn()
        {
            while (true)
            {
                // 执行命令
                yield return commandsController.ExcuteCommand();
                // 检查战斗是否结束
                if (IsBattleOver)
                {
                    yield break;
                }

                // 当前实体正在行动，等待其行动结束
                if (_currentActEntity == null || !_currentActEntity.CanAct)
                {
                    // 排序位置
                    SortOrder();
                    // 更新当前行动实体
                    UpdateActEntity();
                    // 启用当前实体行动
                    _currentActEntity.ExecuteAction();
                }

                yield return null;
            }
        }

        /// <summary>
        /// 更新当前行动实体
        /// </summary>
        private void UpdateActEntity()
        {
            // 再让下一个实体行动
            _currentActEntity = _context.GetAllBattleEntity()[0];
            // 更新当前实体
            _context.SetCurrentEntity(_currentActEntity);
            // 更新实体看向
            UpdateEntityLookAt(_currentActEntity);
        }

        /// <summary>
        /// 更新实体看向
        /// </summary>
        /// <param name="target"></param>
        public void UpdateEntityLookAt(IBattleEntityObject target)
        {
            if (target is PlayerObject)
            {
                //Transform playerTrans = BattlePoint.Instance.GetPlayerTransByIndex(target.EntityPosIndex);
                //Vector3 newPlayerPos = new Vector3(playerTrans.position.x, 0, playerTrans.position.z);
                //LogManager.Log($"玩家位置索引：{target.EntityPosIndex}；玩家位置；{newPlayerPos}");

                // 所有怪物看向当前玩家
                //IEnumerable<Transform> monsterTrans = BattlePoint.Instance.GetMonsterTransforms();
                //foreach (var trans in monsterTrans)
                //{
                //    Vector3 newtransPos = new Vector3(trans.position.x, 0, trans.position.z);
                //    // 计算怪物在世界空间中需要的目标旋转（朝向玩家）
                //    //trans.rotation = Quaternion.LookRotation(newPlayerPos - newtransPos);
                //    Quaternion parentWorldRot = trans.parent.rotation;
                //    trans.localRotation = Quaternion.Inverse(parentWorldRot) * Quaternion.LookRotation(newPlayerPos - newtransPos);
                //    LogManager.Log($"怪物位置索引：{target.EntityPosIndex}；怪物位置；{newtransPos}");
                //}
            }
            else if (target is MonsterObject)
            {
                // 假设是单体攻击，怪物攻击哪个玩家，就激活哪个玩家的摄像机
            }
        }

        /// <summary>
        /// 初始化顺序
        /// 用于选取第一个行动的实体
        /// </summary>
        private void InitOrder()
        {
            // 初始化所有角色的行动值
            foreach (IBattleEntityObject battleEntityObject in _context.GetAllBattleEntity())
            {
                // 初始化行动值
                battleEntityObject.SetActionValue(CalcActionValue(battleEntityObject.GetSpeed()));
            }

            // 基于行动值初始化行动顺序
            _context.GetAllBattleEntity().Sort((c1, c2) =>
            {
                // 比较行动值确定行动顺序。行动值低，越先行动
                if (c1.ActionValue < c2.ActionValue)
                {
                    return -1;
                }
                else if (c1.ActionValue > c2.ActionValue)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });

            // TODO：暂时这样处理：第一个行动的实体行动值为0，后续可能根据算法优化
            _context.GetFirstBattleEntity().SetActionValue(0);
            // 事件分发传递，更新行动轴UI显示
            _context.GetEventBus().TriggerEvent(new ActionBarSortPostEvent(_context, _context.GetLiveEntitys()));
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
            _context.GetAllBattleEntity().Remove(_currentActEntity);

            int toatalSpeed = 0;
            // 重新计算剩下实体各自的剩余行动值
            foreach (IBattleEntityObject battleEntityObject in _context.GetLiveEntitys())
            {
                toatalSpeed += battleEntityObject.GetSpeed();
            }

            foreach (IBattleEntityObject battleEntityObject in _context.GetLiveEntitys())
            {
                float oldAV = battleEntityObject.ActionValue;
                float newAV = (1 - battleEntityObject.GetSpeed() / (float)toatalSpeed) * oldAV;
                battleEntityObject.SetActionValue(newAV);
            }

            // 基于行动值初始化行动顺序
            _context.GetAllBattleEntity().Sort((c1, c2) =>
            {
                // 比较行动值确定行动顺序。行动值低，越先行动
                if (c1.ActionValue < c2.ActionValue)
                {
                    return -1;
                }
                else if (c1.ActionValue > c2.ActionValue)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });

            InsertOrder(_currentActEntity);
            _context.GetAllBattleEntity()[0].SetActionValue(0);

            // 事件分发传递，更新行动轴UI显示
            _context.GetEventBus().TriggerEvent(new ActionBarSortPostEvent(_context, _context.GetLiveEntitys()));
        }

        /// <summary>
        /// 插入队列
        /// </summary>
        /// <param name="actEndEntity"></param>
        public void InsertOrder(IBattleEntityObject actEndEntity)
        {
            actEndEntity.SetActionValue(CalcActionValue(actEndEntity.GetSpeed()));
            int index = _context.GetAllBattleEntity().FindIndex(battleEntity => battleEntity.ActionValue > actEndEntity.ActionValue);
            if (index != -1)
            {
                // 找到第一个行动值大于当前角色的索引，插入到该位置前
                _context.GetAllBattleEntity().Insert(index, actEndEntity);
            }
            else
            {
                // 所有角色行动值都更小，插入末尾
                _context.GetAllBattleEntity().Add(actEndEntity);
            }
        }

        /// <summary>
        /// 计算行动值
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        private float CalcActionValue(float speed)
        {
            // 计算行动值，基准行动值 / 速度 * 修正系数
            return BASE_ACTION_VALUE / speed * SPEED_CORRECTION;
        }

        /// <summary>
        /// 检查战斗是否结束
        /// </summary>
        /// <returns></returns>
        public bool CheckBattleOver()
        {
            // 每次执行完命令后，检查战斗是否结束
            IsBattleOver = battleOverCondition.CheckOver(_context);
            return IsBattleOver;
        }

        /// <summary>
        /// 移除死亡怪物实体
        /// </summary>
        public IEnumerator RemoveDeadMonster()
        {
            var deadEntitys = _context.GetDeadEntitys();
            foreach (var battleEntity in deadEntitys)
            {
                yield return battleEntity.Die();
                _context.GetAllBattleEntity().Remove(battleEntity);
                if (battleEntity == _currentActEntity)
                {
                    _currentActEntity = null;
                }
                if (battleEntity is MonsterObject)
                {
                    GameObject.Destroy(battleEntity.GameObject);
                }
            }

            // 事件分发传递，更新行动轴UI显示
            _context.GetEventBus().TriggerEvent(new ActionBarSortPostEvent(_context, _context.GetLiveEntitys()));
        }

        /// <summary>
        /// 插入命令
        /// </summary>
        /// <param name="skill"></param>
        public void InsertCommand(ICommand command)
        {
            commandsController.InsertCommand(command);
        }

        /// <summary>
        /// 战斗结束
        /// </summary>
        private void BattleOver()
        {
            // 切换为正常倍速
            TimerManager.Instance.SetTimeRate(E_TimeRate.Normal);
            // 触发战斗结束事件
            _context.GetEventBus().TriggerEvent(new BattleOverEvent(_context));
        }
    }
}
