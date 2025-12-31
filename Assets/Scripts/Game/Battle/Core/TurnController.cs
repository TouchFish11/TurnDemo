using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        // 战斗实体列表
        private List<IBattleEntityObject> battleEntities;
        // 技能命令队列
        private readonly Queue<ISkill> skillCommands = new Queue<ISkill>();
        // 当前行动实体
        private IBattleEntityObject _currentActEntity;
        //当前战斗阶段
        private E_BattlePhase _battlePhase = E_BattlePhase.None;

        /// <summary>
        /// 基础行动值
        /// </summary>
        private const float BASE_ACTION_VALUE = 10000f;
        /// <summary>
        /// 速度修正系数（平衡不同速度区间）
        /// </summary>
        private const float SPEED_CORRECTION = 1.0f;

        public TurnController(IBattleContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 初始化行动
        /// </summary>
        /// <param name="battleEntityObjects"></param>
        public void InitActions(IEnumerable<IBattleEntityObject> battleEntityObjects)
        {
            battleEntities = new List<IBattleEntityObject>(battleEntityObjects);
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
        }

        /// <summary>
        /// 战斗准备
        /// </summary>
        private async Task BattlePreparation()
        {
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
            _currentActEntity = battleEntities[0];
        }

        /// <summary>
        /// 初始化顺序
        /// </summary>
        private async void InitOrder()
        {
            // 初始化所有角色的行动值
            foreach (IBattleEntityObject battleEntityObject in battleEntities)
            {
                // 初始化行动值
                battleEntityObject.SetActionValue(CalcActionValue(battleEntityObject.GetSpeed()));
            }

            // 基于行动值初始化行动顺序
            battleEntities.Sort((c1, c2) =>
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

            battleEntities[0].SetActionValue(0);

            // 更新行动轴UI显示
            // TODO：暂时直接调用界面方法，后续通过事件分发传递
            BattleController battleController = UIManager.Instance.GetView<BattleController>();
            await battleController.UpadteActionBar(battleEntities);
        }

        /// <summary>
        /// 排序顺序
        /// </summary>
        private async void SortOrder()
        {
            // 暂时移除第一个角色，不参与计算
            battleEntities.Remove(_currentActEntity);

            int toatalSpeed = 0;
            // 重新计算剩下实体各自的剩余行动值
            foreach (IBattleEntityObject battleEntityObject in battleEntities)
            {
                toatalSpeed += battleEntityObject.GetSpeed();
            }

            foreach (IBattleEntityObject battleEntityObject in battleEntities)
            {
                float oldAV = battleEntityObject.ActionValue;
                float newAV = (1 - battleEntityObject.GetSpeed() / (float)toatalSpeed) * oldAV;
                battleEntityObject.SetActionValue(newAV);
            }

            // 基于行动值初始化行动顺序
            battleEntities.Sort((c1, c2) =>
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
            battleEntities[0].SetActionValue(0);

            // 更新行动轴UI显示
            // TODO：暂时直接调用界面方法，后续通过事件分发传递
           BattleController battleController = UIManager.Instance.GetView<BattleController>();
           await battleController.UpadteActionBar(battleEntities);
        }

        /// <summary>
        /// 插入队列
        /// </summary>
        /// <param name="actEndEntity"></param>
        public async void InsertOrder(IBattleEntityObject actEndEntity)
        {
            actEndEntity.SetActionValue(CalcActionValue(actEndEntity.GetSpeed()));
            int index = battleEntities.FindIndex(battleEntity => battleEntity.ActionValue > actEndEntity.ActionValue);
            if (index != -1)
            {
                // 找到第一个行动值大于当前角色的索引，插入到该位置前
                battleEntities.Insert(index, actEndEntity);
            }
            else
            {
                // 所有角色行动值都更小，插入末尾
                battleEntities.Add(actEndEntity);
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
            // _actions.AddFirst(_actions.Find(battleEntity));
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
