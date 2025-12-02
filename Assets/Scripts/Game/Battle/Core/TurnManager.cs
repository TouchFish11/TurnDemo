using Framework;
using GameLogic.BattleMoudule.Entity;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Game.Battle
{
    /// <summary>
    /// 战斗核心流程：回合管理器（负责回合推进，仅依赖事件总线）
    /// </summary>
    public class TurnManager
    {
        // 战斗上下文
        private IBattleContext _context;
        // 行动链表（按速度排序）
        private List<IBattleEntityObject> _actionList;
        // 当前行动实体
        private IBattleEntityObject _currentActEntity;
        //当前战斗阶段
        private E_BattlePhase _battlePhase;

        public TurnManager(IBattleContext context)
        {
            _context = context;
            _actionList = new List<IBattleEntityObject>(context.GetAllBattleEntity());
            _actionList.Sort(SortActionBySpeed);
        }

        /// <summary>
        /// 战斗循环
        /// </summary>
        /// <returns></returns>
        public IEnumerator BattleLoop()
        {
            while (_battlePhase != E_BattlePhase.QuitBattle)
            {
                switch (_battlePhase)
                {
                    case E_BattlePhase.Preparation:
                        yield return BattlePreparation();
                        break;
                    case E_BattlePhase.EntityTurn:
                        yield return ActEntityTurn();
                        break;
                    case E_BattlePhase.BattleOver:
                        BattleOver();
                        break;
                }
                yield return null;
            }
        }

        /// <summary>
        /// 战斗准备
        /// </summary>
        private async Task BattlePreparation()
        {
            BattleView battlePanel = null;
            //显示战斗UI、播放入场动画等
            await UIManager.Instance.ShowViewAsync<BattleView, BattleModel, BattleController>(E_UILayer.Mid);

            //更新UI显示
            battlePanel.InitUI(_actionList);
            //更新敌人相关UI
            battlePanel.InitMonsterUI(_actionList);
            //更新玩家UI
            battlePanel.InitPlayerObjUI(_actionList);
            //设置为角色行动阶段
            _battlePhase = E_BattlePhase.EntityTurn;
        }

        /// <summary>
        /// 实体行动回合
        /// </summary>
        private IEnumerator ActEntityTurn()
        {
            IBattleEntityObject currentEntity = _actionList[0];

            BattleComponent battleComponent = currentEntity.GetComponent<BattleComponent>();
            //获取当前行动的角色
            while (currentEntity == null || battleComponent.IsDeath || currentEntity != _actionList[0])
            {
                currentEntity = _actionList[0];
            }

            //改变标识
            _battlePhase = E_BattlePhase.Waiting;
            //等待实体行动完毕
            yield return currentEntity.ExecuteAction();

            //设置为未选中
            //for (int i = 0; i < actionableObjs.Count; i++)
            //{
            //    actionableObjs[i].SetSelectFlag(false);
            //}

            //战斗结束，退出循环
            if (_battlePhase != E_BattlePhase.BattleOver)
            {
                //切换到下一个目标行动
                SortOrder();

                //改变阶段
                _battlePhase = E_BattlePhase.EntityTurn;
            }
            else
            {
                yield break;
            }
        }

        /// <summary>
        /// 排序顺序
        /// </summary>
        public void SortOrder(/*List<IBattleTarget> playerTeam, List<IBattleTarget> enemyTeam*/)
        {
            // 测试，行动完放在最后
            IBattleEntityObject currentEntity = _actionList[0];
            _actionList.RemoveAt(0);
            _actionList.Add(currentEntity);

            // TODO：基于速度排序
            ////分别存储玩家、敌人角色
            //_playerCharacters.AddRange(playerTeam);
            //_monsterCharacters.AddRange(enemyTeam);

            ////存储所有可行动对象
            //_actionableObjs.AddRange(playerTeam);
            //_actionableObjs.AddRange(enemyTeam);

            ////初始化所有角色的行动值
            //for (int i = 0; i < _actionableObjs.Count; i++)
            //{
            //    _actionableObjs[i].SetActionValue(Distance);
            //}

            ////基于行动值初始化行动顺序
            //_actionableObjs.Sort((c1, c2) =>
            //{
            //    //比较行动值确定行动顺序
            //    // 行动值低，越先行动
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
        /// 战斗结束
        /// </summary>
        private async void BattleOver()
        {
            //显示战斗结束UI
            //隐藏战斗UI
            BattleController battleController = UIManager.Instance.GetView<BattleView, BattleModel, BattleController>();

            //切换为正常倍速
            TimerMgr.Instance.SetTimeRate(E_TimeRate.Normal);
            battleController.BattleOver();

            //切换场景
            await SceneManager.Instance.LoadSceneAsync("MainScene", UnityEngine.SceneManagement.LoadSceneMode.Single, (progress) =>
            {

            });

            //清空缓存池
            PoolManager.Instance.Clear();
            //显示主界面
           await UIManager.Instance.ShowViewAsync<MainView, MainModel, MainController>(E_UILayer.Top);
            //改变阶段
            _battlePhase = E_BattlePhase.QuitBattle;
        }

        /// <summary>
        /// 插入到行动头
        /// </summary>
        /// <param name="battleEntity"></param>
        public void InsertToActionHead(IBattleEntityObject battleEntity)
        {
            _actionList.Insert(0, battleEntity);
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
