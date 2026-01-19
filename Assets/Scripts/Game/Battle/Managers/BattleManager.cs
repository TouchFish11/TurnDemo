using Framework;
using System.Threading.Tasks;
using Game.Battle.Core;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 战斗管理器
    /// </summary>
    public class BattleManager : SingletonBase<BattleManager>, IBattleManager
    {
        // 战斗上下文
        private IBattleContext context;

        private BattleManager()
        {

        }

        public async Task StartBattle()
        {
            // 创建战斗加载界面
            BattleLoadingController battleLoadingController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<BattleLoadingView, BattleLoadingModel, BattleLoadingController>(E_UILayer.Mid);
            // 清理场景内容缓存
            Main.Instance.ClearScene();
            // 加载战斗场景
            ServiceLocator.Get<ISceneManager>().LoadSceneAsync(ResKeyCollection.LevelScene, UnityEngine.SceneManagement.LoadSceneMode.Single, 
            (progress) => battleLoadingController.UpdateProgress(progress), 
            async () =>
            {
                // 销毁战斗加载界面
                ServiceLocator.Get<IUIManager>().DestroyView();
                // 销毁主界面
                ServiceLocator.Get<IUIManager>().DestroyView();
                // 初始化战斗上下文
                context = new BattleContext();
                // 初始化战斗
                InitBattle();
                // 战斗上下文的战斗初始化
                await context.InitBattle();
                // 初始化
                BattlePoint.Instance.InitBattlePoint(context, context.GetLivePlayerObjects());
                // 开始战斗循环
                ServiceLocator.Get<IMonoManager>().StartCoroutine(context.GetTurnManager().BattleLoop());
            });
        }

        public IBattleContext GetContext()
        {
            return context;
        }

        private void InitBattle()
        {
            ServiceLocator.Get<ITargetSelectManager>().Init(context);
            // IDamageCalcManager 依赖ITargetSelectManager
            ServiceLocator.Get<IDamageCalcManager>().Init(context);
            // 监听战斗退出事件
            context.GetEventBus().AddListener<QuitBattleEvent>(OnQuitBattleEvent);
        }

        /// <summary>
        /// 战斗退出事件回调
        /// </summary>
        /// <param name="quitBattleEvent"></param>
        private void OnQuitBattleEvent(QuitBattleEvent quitBattleEvent)
        {
            // 销毁战斗界面
            ServiceLocator.Get<IUIManager>().DestroyView();
            // 清理战斗数据
            context.CleanupBattle();
            // 销毁战斗输入处理器、战斗点对象、战斗UI调度器
            GameObject.Destroy(BattleInputHandler.Instance.gameObject);
            GameObject.Destroy(BattlePoint.Instance.gameObject);
            GameObject.Destroy(BattleUIScheduler.Instance.gameObject);
            // 显示黑背景
            ShowBackView();
        }

        private async void ShowBackView()
        {
            // 创建黑背景界面遮挡
            BackController backController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<BackView, BackModel, BackController>(E_UILayer.Mid);
            ServiceLocator.Get<ISceneManager>().LoadSceneAsync(ResKeyCollection.MainScene, UnityEngine.SceneManagement.LoadSceneMode.Single, (progress) =>
            {
                // 不需要显示进度
            }, 
            async () =>
            {
                // 销毁黑背景界面
                ServiceLocator.Get<IUIManager>().DestroyView();
                // 销毁战斗界面
                ServiceLocator.Get<IUIManager>().DestroyView();
                // 初始化场景
                Main.Instance.InitScene();
                await Task.CompletedTask;
            });
        }
    }
}
