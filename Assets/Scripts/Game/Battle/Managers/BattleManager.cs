using Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Battle
{
    /// <summary>
    /// 战斗管理器
    /// </summary>
    public class BattleManager : SingletonBase<BattleManager>, IBattleManager
    {
        // 敌人数量
        private int _monsterNum;
        // 战斗上下文
        private IBattleContext context;
        // 实体战斗点
        private BattlePoint battlePoint; 

        private BattleManager()
        {

        }

        public async Task StartBattle(/* 战斗角色选择，怪物选择，战斗场景选择（可选）， */)
        {
            // 初始化战斗上下文
            context = new BattleContext();
            // 初始化战斗相关管理器
            InitBattle();
            // 初始化战斗
            await context.InitBattle();
            // 获取场景上的战斗点对象，初始化战斗点对象
            battlePoint = BattlePoint.Instance.InitBattlePoint();
            // 启动回合
            MonoManager.Instance.StartCoroutine(context.GetTurnManager().BattleLoop());
        }

        public IBattleContext GetContext()
        {
            return context;
        }

        private void InitBattle()
        {
            // 依赖战斗上下文
            ServiceLocator.Register<ITargetSelectManager>(TargetSelectManager.Instance);
            // 被玩家创建时依赖，所以要先于玩家创建
            ServiceLocator.Register<IDamageCalcManager>(DamageCalcManager.Instance);
            ServiceLocator.Register<ISkillManager>(SkillManager.Instance);

            // 监听退出战斗事件
            context.GetEventBus().AddListener<QuitBattleEvent>(OnQuitBattleEvent);
        }

        private void OnQuitBattleEvent(QuitBattleEvent quitBattleEvent)
        {
            // 清理战斗
            context.CleanupBattle();
            // 显示黑背景
            ShowBackView();
        }

        private async void ShowBackView()
        {
            BackController backController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<BackView, BackModel, BackController>(E_UILayer.Mid);
            LogManager.Log($"显示黑背景");
            //backController.CompletedHide(() =>
            //{
            //    //切换场景
            //    SceneManager.Instance.LoadSceneAsync(ResKeyCollection.MainScene, UnityEngine.SceneManagement.LoadSceneMode.Single, (progress) =>
            //    {
            //        LogManager.Log($"加载进度：{progress}");
            //    }, async () =>
            //    {
            //        // 隐藏背景
            //        ServiceLocator.Get<IUIManager>().DestroyView();
            //        // 显示主界面
            //        await ServiceLocator.Get<IUIManager>().CreateViewAsync<MainView, MainModel, MainController>(E_UILayer.Top);
            //    });
            //});
        }
    }
}
