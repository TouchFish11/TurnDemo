using Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Battle
{
    /// <summary>
    /// 战斗管理器
    /// </summary>
    public class BattleManager : SingletonBase<BattleManager>
    {
        // 敌人数量
        private int _monsterNum;
        // 战斗上下文
        private IBattleContext context;

        private BattleManager()
        {

        }

        /// <summary>
        /// 启动战斗（外部调用：如主界面按钮点击）
        /// </summary>
        public async Task StartBattle(/* 战斗角色选择，怪物选择，战斗场景选择（可选）， */)
        {
            BattleController battleController = await UIManager.Instance.CreateViewAsync<BattleView, BattleModel,BattleController>(E_UILayer.Mid);
            // 初始化战斗上下文
            context = new BattleContext();
            // 初始化战斗
            await context.InitBattle();
            // 更新战斗UI、播放入场动画等
            await battleController.InitBattleUI(context);
            // 启动回合
            MonoManager.Instance.StartCoroutine(context.GetTurnManager().BattleLoop());
        }

        public IBattleContext GetContext()
        {
            return context;
        }
    }
}
