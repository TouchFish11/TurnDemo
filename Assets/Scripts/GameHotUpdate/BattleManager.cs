using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Config;
using Core.DataPersistence.Binary;
using Core.Mono;
using Core.Scene;
using Core.Service;
using Core.Singleton;
using Core.UI;
using Core.UI.MVC;
using Game.Battle;
using Game.Battle.Context;
using Game.Battle.Damage;
using Game.Battle.Input;
using Game.Battle.Objects;
using Game.Battle.Skill.Base;
using Game.Battle.Skill.Interface;
using Game.Battle.TargetSelect;
using Game.Input;
using Game.UI.Battle;
using GameHotUpdate.Animation;
using GameHotUpdate.Battle;
using GameHotUpdate.Battle.Damage;
using GameHotUpdate.Battle.Event.Turn;
using GameHotUpdate.Battle.Object;
using GameHotUpdate.Context;
using GameHotUpdate.Input;
using GameHotUpdate.Main;
using GameHotUpdate.Objects;
using GameHotUpdate.TargetSelect;
using GameHotUpdate.UI.Back;
using GameHotUpdate.UI.Loading.Battle;
using UnityEngine;

namespace GameHotUpdate
{
    /// <summary>
    /// 战斗管理器
    /// </summary>
    public class BattleManager : SingletonBase<BattleManager>, IBattleManager
    {
        // 战斗上下文
        private IBattleContext _context;

        private BattleManager()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private static void RegisterManager(IBattleContext context)
        {
            ServiceLocator.Register<ITargetSelectManager>(TargetSelectManager.Instance);
            ServiceLocator.Get<ITargetSelectManager>().Init(context);
            
            // IDamageCalcManager 依赖 ITargetSelectManager
            ServiceLocator.Register<IDamageCalcManager>(DamageCalcManager.Instance);
            ServiceLocator.Get<IDamageCalcManager>().Init(context);
            
            ServiceLocator.Register<IBattleInputHandler>(BattleInputHandler.Instance);
            ServiceLocator.Get<IBattleInputHandler>().Init(context);
            
            ServiceLocator.Register<ISkillManager>(SkillManager.Instance);
            ServiceLocator.Register<IAnimationPlayManager>(AnimationPlayManager.Instance);
            
            // 注册战斗点，依赖战斗场景加载完成
            ServiceLocator.Register<IBattlePoint>(BattlePoint.Instance);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void UnregisterManager()
        {
            ServiceLocator.Unregister<ITargetSelectManager>();
            ServiceLocator.Unregister<IDamageCalcManager>();
            ServiceLocator.Unregister<ISkillManager>();
            ServiceLocator.Unregister<IAnimationPlayManager>();
            ServiceLocator.Unregister<IBattleInputHandler>();
            ServiceLocator.Unregister<IBattlePoint>();
            ServiceLocator.Unregister<IBattleUIScheduler>();
        }

        public async Task StartBattle(IuiController controller)
        {
            // 创建战斗加载界面
            var battleLoadingController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<BattleLoadingView, BattleLoadingModel, BattleLoadingController>(E_UILayer.Bot, ResKeyCollection.BattleLoadingView);
            
            // 清理场景内容缓存
            HotfixGameMain.ClearScene();
            
            // 加载战斗场景
            ServiceLocator.Get<ISceneManager>().LoadSceneAsync(ResKeyCollection.LevelScene, UnityEngine.SceneManagement.LoadSceneMode.Single, 
            (progress) => battleLoadingController.UpdateProgress(progress), 
            async () =>
            {
                // 销毁主界面
                ServiceLocator.Get<IUIManager>().DestroyView(controller);

                // 创建战斗上下文
                _context = new BattleContext();
                // 监听战斗退出事件
                _context.GetEventBus().AddListener<QuitBattleEvent>(OnQuitBattleEvent);

                // 注册管理器
                RegisterManager(_context);
                // 创建战斗实体对象，依赖战斗上下文、战斗点
                await CreateBattleEntity();

                // 初始化战斗点，依赖战斗实体对象创建完成
                ServiceLocator.Get<IBattlePoint>().InitBattlePoint(_context, new List<IBattleEntityObject>(_context.GetAlivePlayerEntitys()));
                
                // 进入战斗准备
                await _context.GetTurnManager().BattlePreparation();
                // 战斗准备完毕，销毁战斗加载界面
                ServiceLocator.Get<IUIManager>().DestroyView(battleLoadingController);
                // 开始战斗
                ServiceLocator.Get<IMonoManager>().StartCoroutine(_context.GetTurnManager().StartBattle());
            });
        }
        
        /// <summary>
        /// TODO：可优化为使用战斗实体创建器来创建怪物、波次
        /// 创建战斗实体对象
        /// </summary>
        /// <returns></returns>
        private async Task CreateBattleEntity()
        {
            var playerTrans = new List<Transform>(ServiceLocator.Get<IBattlePoint>().GetPlayerTransforms());
            // 批量创建玩家角色（从配置+预制体）
            var playerDataDic = ServiceLocator.Get<IBinaryDataManager>().GetConfig<RoleInfoContainer>(EConfigLoadType.Editor).dataDic;
            var index = 0;
            foreach (var roleId in playerDataDic.Keys)
            {
                if (index == playerTrans.Count)
                {
                    break;
                }

                var transform = playerTrans[index];

                var hotfixPlayerObject = await RoleBuilder.CreateRole(roleId, transform);
                // 注入上下文，供角色内部组件使用
                hotfixPlayerObject.BattleInit(roleId, _context);
                // 记录角色所在的位置索引
                hotfixPlayerObject.EntityPosIndex = index;
                _context.AddBattleEntity(hotfixPlayerObject);
                index++;
            }

            // 批量创建怪物角色（从配置+预制体）
            const int monsterCount = 1;
            var monsterTrans = new List<Transform>(BattlePoint.Instance.GetMonsterTransforms());
            var keys = new List<int>(ServiceLocator.Get<IBinaryDataManager>().GetConfig<MonsterInfoContainer>(EConfigLoadType.Editor).dataDic.Keys);
            index = 0;
            while (index < monsterCount)
            {
                var transform = monsterTrans[index];
                var monsterId = keys[Random.Range(0, keys.Count)];
                var hotfixMonsterObject = await MonsterBuilder.CreateMonster(monsterId, transform);
                // 注入上下文，供角色内部组件使用
                hotfixMonsterObject.BattleInit(monsterId, _context);
                // 记录怪物所在的位置索引
                hotfixMonsterObject.EntityPosIndex = index;
                _context.AddBattleEntity(hotfixMonsterObject);
                index++;
            }
        }

        public IBattleContext GetContext()
        {
            return _context;
        }

        /// <summary>
        /// 战斗退出事件回调
        /// </summary>
        /// <param name="quitBattleEvent"></param>
        private void OnQuitBattleEvent(QuitBattleEvent quitBattleEvent)
        {
            // 清理战斗数据
            _context.CleanupBattle();
            // 销毁战斗输入处理器、战斗点对象、战斗UI调度器
            Object.Destroy(ServiceLocator.Get<IBattleInputHandler>().GameObject);
            Object.Destroy(ServiceLocator.Get<IBattlePoint>().GameObject);
            Object.Destroy(ServiceLocator.Get<IBattleUIScheduler>().GameObject);
            
            // 移除注册
            UnregisterManager();
            
            // 销毁战斗界面
            ServiceLocator.Get<IUIManager>().DestroyView(quitBattleEvent.BattleUIController);
            BackMain();
        }

        /// <summary>
        /// 回到主场景
        /// </summary>
        private static async void BackMain()
        {
            // 创建黑背景界面遮挡
            var backController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<BackView, BackModel, BackController>(E_UILayer.Bot, ResKeyCollection.BackView);
            
            ServiceLocator.Get<ISceneManager>().LoadSceneAsync(ResKeyCollection.MainScene, UnityEngine.SceneManagement.LoadSceneMode.Single, null, 
            async () =>
            {
                // 初始化场景
                await HotfixGameMain.InitScene();
                // 销毁黑背景界面
                ServiceLocator.Get<IUIManager>().DestroyView(backController);
                // 强制不可见，暂时这样处理，正常流程Bug：battleLoadingController销毁时未正确释放
                ServiceLocator.Get<IMouseManager>().ForceInVisible();
                await Task.CompletedTask;
            });
        }
    }
}
