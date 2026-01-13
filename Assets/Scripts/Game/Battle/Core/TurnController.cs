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
        // 当前战斗阶段
        private E_BattlePhase _battlePhase = E_BattlePhase.None;
        // 当前战斗结束条件
        private IBattleOverCondition battleOverCondition;
        // 当前怪物数量
        private int currentMonsterCount;

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
        /// 初始化行动
        /// </summary>
        /// <param name="battleEntityObjects"></param>
        public void InitActions()
        {
            _battlePhase = E_BattlePhase.Preparation;
            currentMonsterCount = _context.GetMonsterObjects().Count;
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
            // 设置为角色行动阶段
            _battlePhase = E_BattlePhase.EntityTurn;
        }

        /// <summary>
        /// 实体行动回合
        /// </summary>
        private IEnumerator ActEntityTurn()
        {
            // 等待战斗结束
            _battlePhase = E_BattlePhase.WaitingBattleOver;
            while (true)
            {
                // 执行命令
                yield return commandsController.ExcuteCommand();
                // 检查战斗是否结束
                if (CheckBattleOver())
                {
                    LogManager.Log($"战斗结束，退出循环");
                    yield break;
                }

                // 当前实体正在行动，等待其行动结束
                if (!_currentActEntity.CanAct)
                {
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
            if (_currentActEntity != null)
            {
                SortOrder();
            }

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
                // 当前玩家看向存活怪物中心
                Vector3 center = GetLiveMonstersCenter();
                Transform playerTrans = BattlePoint.Instance.GetPlayerTransByIndex(target.EntityPosIndex);

                Vector3 playerPos = playerTrans.position;
                Vector3 newPlayerPos = new Vector3(playerPos.x, 0, playerPos.z);
                playerTrans.rotation = Quaternion.LookRotation(center - newPlayerPos);

                // 所有怪物看向当前玩家
                IEnumerable<Transform> monsterTrans = BattlePoint.Instance.GetMonsterTransforms();
                foreach (var trans in monsterTrans)
                {
                    Vector3 transPos = trans.position;
                    Vector3 newtransPos = new Vector3(transPos.x, 0, transPos.z);
                    trans.rotation = Quaternion.LookRotation(newPlayerPos - newtransPos);
                }
            }
            else if (target is MonsterObject)
            {
                // 假设是单体攻击，怪物攻击哪个玩家，就激活哪个玩家的摄像机


            }
        }

        // 获取存活怪物中心
        private Vector3 GetLiveMonstersCenter()
        {
            List<IBattleEntityObject> monsters = new List<IBattleEntityObject>(_context.GetMonsterObjects());

            int leftIndex = monsters[0].EntityPosIndex;
            int rightIndex = monsters[monsters.Count - 1].EntityPosIndex;

            Vector3 leftPos = BattlePoint.Instance.GetMonsterTransByIndex(leftIndex).position;
            Vector3 rightPos = BattlePoint.Instance.GetMonsterTransByIndex(rightIndex).position;

            Vector3 center = (leftPos + rightPos) / 2;

            return new Vector3(center.x, 0, center.z);
        }

        /// <summary>
        /// 初始化顺序
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

            _context.GetFirstBattleEntity().SetActionValue(0);
            // 事件分发传递，更新行动轴UI显示
            _context.GetEventBus().TriggerEvent(new ActionBarSortPostEvent(_context, _context.GetLiveEntitys()));
        }

        /// <summary>
        /// 排序顺序
        /// </summary>
        private void SortOrder()
        {
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
            if (battleOverCondition.CheckOver(_context))
            {
                // 战斗结束，退出循环
                _battlePhase = E_BattlePhase.BattleOver;
                return true;
            }
            else
            {
                // 改变阶段
                _battlePhase = E_BattlePhase.EntityTurn;
                return false;
            }
        }

        /// <summary>
        /// 移除死亡怪物实体
        /// </summary>
        public void RemoveDeadMonster()
        {
            var deadMonsters = _context.GetDeadMonsterEntitys();
            foreach (var deadMonster in deadMonsters)
            {
                _context.GetAllBattleEntity().Remove(deadMonster);
                GameObject.Destroy(deadMonster.GameObject);
            }

            // 事件分发传递，更新行动轴UI显示
            _context.GetEventBus().TriggerEvent(new ActionBarSortPostEvent(_context, _context.GetLiveEntitys()));
        }

        /// <summary>
        /// 更新怪物实体位置
        /// 切换相机时更新
        /// </summary>
        public void UpdateMonsterEntityPoses()
        {
            int newCount = _context.GetMonsterObjects().Count;
            if (currentMonsterCount == newCount)
            {
                return;
            }
            currentMonsterCount = newCount;

            List<Transform> monsterTrans = new List<Transform>(BattlePoint.Instance.GetMonsterTransforms());
            var monsters = _context.GetMonsterObjects();
            monsters.Sort((m1, m2) =>
            {
                if (m1.EntityPosIndex < m2.EntityPosIndex)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });
            for (int i = 0; i < monsters.Count; i++)
            {
                // 更新位置索引
                (monsters[i] as MonsterObject).EntityPosIndex = i;
                // 设置父对象
                monsters[i].GameObject.transform.SetParent(monsterTrans[i], false);
            }

            //// 移动对齐剩下的怪物位置
            //int index = battleEntity.EntityPosIndex;
            //if (index == 0 || index == monsterTrans.Count - 1)
            //{
            //    // 不用处理
            //}
            //else if (index == 1)
            //{
            //    // 获取0索引怪物
            //    IBattleEntityObject target = GetMonsterByIndex(0);
            //    // 移动最左侧的怪物到1的位置
            //    target.GameObject.transform.SetParent(BattlePoint.Instance.GetMonsterTransByIndex(index), false);
            //}
            //else if (index == monsterTrans.Count - 2)
            //{
            //    // 获取最后索引怪物
            //    IBattleEntityObject target = GetMonsterByIndex(monsterTrans.Count - 1);
            //    // 移动最右侧的怪物到指定的位置
            //    target.GameObject.transform.SetParent(BattlePoint.Instance.GetMonsterTransByIndex(index), false);
            //}
            //else if (index == monsterTrans.Count / 2)
            //{
            //    IBattleEntityObject target = GetMonsterByIndex(_context.GetAllBattleEntity().Count - 2);
            //    int index2 = target.EntityPosIndex;
            //    target.GameObject.transform.SetParent(BattlePoint.Instance.GetMonsterTransByIndex(index), false);
            //    target = GetMonsterByIndex(_context.GetAllBattleEntity().Count - 1);
            //    target.GameObject.transform.SetParent(BattlePoint.Instance.GetMonsterTransByIndex(index2), false);
            //}
        }

        private IBattleEntityObject GetMonsterByIndex(int index)
        {
            foreach (var monster in _context.GetMonsterObjects())
            {
                if (monster.EntityPosIndex == index)
                {
                    return monster;
                }
            }

            return null;
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
