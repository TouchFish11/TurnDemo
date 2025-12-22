using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Battle
{
    /// <summary>
    /// 战斗核心流程：回合管理器（负责回合推进，仅依赖事件总线）
    /// </summary>
    public class TurnManager
    {
        // 战斗上下文
        private readonly IBattleContext _context;
        // 行动链表（按速度排序）
        private LinkedList<IBattleEntityObject> _actions;
        // 技能命令队列
        private readonly Queue<ISkill> skillCommands = new Queue<ISkill>();
        // 当前行动实体
        private IBattleEntityObject _currentActEntity;
        //当前战斗阶段
        private E_BattlePhase _battlePhase = E_BattlePhase.None;

        /// <summary>
        /// 在回合开始时触发
        /// </summary>
        public event Action<TurnStartEvent> OnTurnStart;

        public TurnManager(IBattleContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 初始化行动
        /// </summary>
        /// <param name="battleEntityObjects"></param>
        public void InitActions(IEnumerable<IBattleEntityObject> battleEntityObjects)
        {
            _actions = new LinkedList<IBattleEntityObject>(battleEntityObjects);
            _battlePhase = E_BattlePhase.Preparation;
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

            //while (_battlePhase != E_BattlePhase.QuitBattle)
            //{
            //    switch (_battlePhase)
            //    {
            //        case E_BattlePhase.Preparation:
            //            yield return TaskUtility.WaitForTask(BattlePreparation());
            //            break;
            //        case E_BattlePhase.EntityTurn:
            //            yield return ActEntityTurn();
            //            break;
            //        case E_BattlePhase.BattleOver:
            //            BattleOver();
            //            break;
            //    }
            //    yield return null;
            //}
        }

        /// <summary>
        /// 战斗准备
        /// </summary>
        private async Task BattlePreparation()
        {
            _context.GetEventBus().AddListener<TurnStartEvent>(OnTurnStart);
            // 初始化行动实体
            UpdateActEntity();
            // 排序行动顺序
            SortOrder();
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
                // 存在命令，执行
                while (skillCommands.Count > 0)
                {
                    // 执行技能命令
                    yield return skillCommands.Dequeue().Cast(_context);

                    // 检查战斗是否结束
                    if (CheckBattleOver())
                    {
                        yield break;
                    }
                }

                // 当前实体正在行动，等待其行动结束
                if (!_currentActEntity.CanAct)
                {
                    // 更新当前行动实体
                    UpdateActEntity();
                    // 排序
                    SortOrder();
                    // 启用当前实体行动
                    _currentActEntity.ExecuteAction();
                }

                yield return null;
            }

            //LinkedListNode<IBattleEntityObject> currentNode = _actions.First;
            //_currentActEntity = currentNode.Value;
            //while (currentNode.Next != null)
            //{
            //    BattleComponent battleComponent = _currentActEntity.GetComponent<BattleComponent>();
            //    // 获取当前行动的角色
            //    if (_currentActEntity == null || battleComponent.IsDeath || _currentActEntity != _actions.First.Value)
            //    {
            //        _currentActEntity = _actions.First.Value;
            //    }
            //    else
            //    {
            //        break;
            //    }
            //    currentNode = currentNode.Next;
            //}

            //// 改变标识
            //_battlePhase = E_BattlePhase.Waiting;

            //// 执行实体回合开始事件
            //BattleController battleController = UIManager.Instance.GetView<BattleController>();
            //_context.GetEventBus().TriggerEvent(new TurnStartEvent(_context, _currentActEntity));

            //// 等待实体行动完毕
            //_currentActEntity.ExecuteAction();

            //设置为未选中
            //for (int i = 0; i < actionableObjs.Count; i++)
            //{
            //    actionableObjs[i].SetSelectFlag(false);
            //}
        }

        /// <summary>
        /// 更新当前行动实体
        /// </summary>
        private void UpdateActEntity()
        {
#if EDITOR_TEST_AB || !UNITY_EDITOR
        targetSelect = new BattleTargetSelect();
#else
            LinkedListNode<IBattleEntityObject> currentNode = _actions.First;
            _currentActEntity = currentNode.Value;
            while (currentNode.Next != null)
            {
                PropertyComponent propertyComponent = _currentActEntity.GetComponent<PropertyComponent>();
                // 获取当前行动的角色
                if (_currentActEntity == null || propertyComponent.IsDeath || _currentActEntity != _actions.First.Value)
                {
                    _currentActEntity = _actions.First.Value;
                }
                else
                {
                    _actions.RemoveFirst();
                    _actions.AddLast(currentNode);
                    break;
                }
                currentNode = currentNode.Next;
            }
#endif
        }

        /// <summary>
        /// 排序顺序
        /// </summary>
        public async void SortOrder()
        {
            // 测试，行动完放在最后
            //LinkedListNode<IBattleEntityObject> currentEntity = _actions.First;
            //_actions.RemoveFirst();
            //_actions.AddLast(currentEntity);

            // 更新行动轴UI显示
            //BattleController battleController = UIManager.Instance.GetView<BattleController>();
            //await battleController.UpadteActionBar(_actions);

            //List<IBattleEntityObject> entityObjects = new List<IBattleEntityObject>(_context.GetAllBattleEntity());
            //// 初始化所有角色的行动值
            //foreach (IBattleEntityObject battleEntityObject in entityObjects)
            //{
            //    battleEntityObject.SetActionValue(-1);
            //}

            //// 基于行动值初始化行动顺序
            //entityObjects.Sort((c1, c2) =>
            //{
            //    // 比较行动值确定行动顺序。行动值低，越先行动
            //    if (c1.ActionValue < c2.ActionValue)
            //    {
            //        return -1;
            //    }
            //    else if (c1.ActionValue > c2.ActionValue)
            //    {
            //        return 1;
            //    }
            //    else
            //    {
            //        return 0;
            //    }
            //});
        }

        /// <summary>
        /// 检查战斗是否结束
        /// </summary>
        /// <returns></returns>
        private bool CheckBattleOver()
        {
            // 每次执行完命令后，检查战斗是否结束
            if (_battlePhase != E_BattlePhase.BattleOver)
            {
                //改变阶段
                _battlePhase = E_BattlePhase.EntityTurn;
                return false;
            }
            else
            {
                // 战斗结束，退出循环
                _battlePhase = E_BattlePhase.BattleOver;
                return true;
            }
        }

        public void EnqueueCommand(ISkill skill)
        {
            skillCommands.Enqueue(skill);
        }

        /// <summary>
        /// 获取当前行动实体
        /// </summary>
        /// <returns></returns>
        public IBattleEntityObject GetCurrentEntity()
        {
            return _currentActEntity;
        }

        /// <summary>
        /// 战斗结束
        /// </summary>
        private async void BattleOver()
        {
            // 显示战斗结束UI
            BattleController battleController = UIManager.Instance.GetView<BattleController>();
            // 切换为正常倍速
            TimerManager.Instance.SetTimeRate(E_TimeRate.Normal);
            battleController.BattleOver();

            //切换场景
            SceneManager.Instance.LoadSceneAsync(ResKeyCollection.MainScene, UnityEngine.SceneManagement.LoadSceneMode.Single, (progress) =>
            {

            }, null);

            // 清空事件总线
            _context.GetEventBus().Clear();
            // 清理战斗
            _context.CleanupBattle();
            // 清空缓存池
            PoolManager.Instance.Clear();
            // 显示主界面
            await UIManager.Instance.CreateViewAsync<MainView, MainModel, MainController>(E_UILayer.Top);
            // 改变阶段
            _battlePhase = E_BattlePhase.QuitBattle;
        }

        /// <summary>
        /// 插入到行动头
        /// </summary>
        /// <param name="battleEntity"></param>
        public void InsertToActionHead(IBattleEntityObject battleEntity)
        {
            _actions.AddFirst(_actions.Find(battleEntity));
        }

        /// <summary>
        /// 通过速度排序行动
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int SortActionBySpeed(IBattleEntityObject a, IBattleEntityObject b)
        {
            if (a.GetSpeed() > b.GetSpeed())
            {
                return -1;
            }
            else if (a.GetSpeed() < b.GetSpeed())
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
