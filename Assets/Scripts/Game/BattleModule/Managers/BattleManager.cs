using Framework;
using GameLogic.BattleMoudule.Core;

namespace GameLogic.BattleMoudule.Managers
{
    /// <summary>
    /// 战斗管理器
    /// </summary>
    public class BattleManager : SingletonBase<BattleManager>
    {
        //敌人数量
        private int _monsterNum;

        private IBattleContext context;

        private BattleManager()
        {

        }

        /// <summary>
        /// 启动战斗（外部调用：如主界面按钮点击）
        /// </summary>
        public void StartBattle(/* 战斗角色选择，怪物选择，战斗场景选择（可选）， */)
        {
            // 加载战斗场景（异步加载，避免卡顿）
            SceneManager.Instance.LoadSceneAsync("", UnityEngine.SceneManagement.LoadSceneMode.Single, (progress) =>
            {

            },
            () =>
            {
                // 初始化战斗上下文
                context = new BattleContext();

                // 创建实体
                (context as BattleContext).CreateEntity();

                // 初始化战斗
                context.InitBattle();
                // 启动回合
                MonoManager.Instance.StartCoroutine(context.GetTurnManager().BattleLoop());
            });
        }
    }
}
