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

        /// <summary>
        /// 启动战斗（外部调用：如主界面按钮点击）
        /// </summary>
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

        private void InitBattle()
        {
            // 依赖战斗上下文
            ServiceLocator.Register<ITargetSelectManager>(TargetSelectManager.Instance);
            // 被玩家创建时依赖，所以要先于玩家创建
            ServiceLocator.Register<IDamageCalcManager>(DamageCalcManager.Instance);
            ServiceLocator.Register<ISkillManager>(SkillManager.Instance);
        }

        /// <summary>
        /// 获取上下文
        /// </summary>
        /// <returns></returns>
        public IBattleContext GetContext()
        {
            return context;
        }
    }
}
