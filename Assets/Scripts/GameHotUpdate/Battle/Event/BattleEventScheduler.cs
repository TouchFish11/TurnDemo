using System.Collections;
using Core.Log;
using Core.Reflection;
using Core.Service;
using Core.Singleton;
using Core.UI;
using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;
using Game.Battle.TargetSelect;
using Game.UI.Battle.SkillKey.Provider;
using GameHotUpdate.Battle.Event.Turn;
using GameHotUpdate.Objects;
using GameHotUpdate.UI.Battle.Base;
using GameHotUpdate.UI.Battle.SkillKey.Provider;
using UnityEngine;

namespace GameHotUpdate.Battle.Event
{
    public class BattleEventScheduler : SingletonAutoMono<BattleEventScheduler>, IBattleEventScheduler
    {
        private IBattleContext _context;

        public GameObject GameObject { get; private set; }

        private void Awake()
        {
            GameObject = gameObject;
        }

        public void Init(IBattleContext context)
        {
            _context = context;
            // 监听战斗事件
            ListenerBattleEvent();
        }
        
        private void ListenerBattleEvent()
        {
            _context.GetEventBus().AddListener<TurnStartEvent>(TurnStartEventScheduler);
        }

        private void TurnStartEventScheduler(TurnStartEvent turnStartEvent)
        {
            if (turnStartEvent.CurrentBattleEntity == null)
            {
                LogManager.LogError($"{nameof(BattleEventScheduler)}.{nameof(TurnStartEventScheduler)}：当前战斗对象为null");
                return;
            }

            if (turnStartEvent.CurrentBattleEntity is PlayerObject)
            {
                // 先执行战斗点位置变化
                _context.GetProxy().UpdateMonsterPos(turnStartEvent.CurrentBattleEntity);
                // 更新相机显示
                _context.GetProxy().UpdateCamera(turnStartEvent.CurrentBattleEntity);
                // 怪物看向玩家
                _context.GetTurnManager().UpdateEntityLookAt(turnStartEvent.CurrentBattleEntity);
            }
            
            // 更新UI
            var controller = ServiceLocator.Get<IUIManager>().GetController<BattleController>();
            controller.UiInitializer.InitMonsterUI(turnStartEvent.Context.GetAliveMonsterEntitys());
            
            switch (turnStartEvent.CurrentBattleEntity)
            {
                case PlayerObject:
                {
                    // 玩家回合：激活目标选择功能
                    ServiceLocator.Get<ITargetSelectManager>().ActiveSelectTarget();
                    // 隐藏行动提示
                    controller.BattleUiManager.SetActTipActive(E_ActTipType.Hide);
                    // 获取技能按键UI数据提供器
                    var provider = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillKeyUIDataProviderFactory, SkillKeyUIDataProviderFactory>().
                        GetCastSkillCondition<BaseSkillKeyUIDataProvider>();
                    
                    // 根据数据更新玩家操作按键，按键触发技能选择事件
                    controller.BattleUiManager.UpdateOperator(turnStartEvent.CurrentBattleEntity, provider);
                    break;
                }
                case MonsterObject:
                    // 怪物回合：关闭目标选择功能
                    ServiceLocator.Get<ITargetSelectManager>().InActiveSelectTarget();
                    // 清除选中目标的标记UI
                    controller.BattleUiManager.ClearSelectMarker();
                    // 清空操作面板
                    controller.BattleUiManager.SetOperator(null);
                    // 显示怪物行动提示
                    controller.BattleUiManager.SetActTipActive(E_ActTipType.Monster);
                    break;
            }
        }

        /// <summary>
        /// 终结技释放前调度逻辑
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skillInfo"></param>
        public IEnumerator PreUltimateCastDispatch(IBattleEntityObject caster, SkillInfo skillInfo)
        {
            // 先执行战斗点位置变化
            _context.GetProxy().UpdateMonsterPos(caster);
            // 更新相机显示
            _context.GetProxy().UpdateCamera(caster);
            // 怪物看向玩家
            _context.GetTurnManager().UpdateEntityLookAt(caster);
            // 玩家回合：激活目标选择功能
            ServiceLocator.Get<ITargetSelectManager>().ActiveSelectTarget();
            
            // 更新UI
            var controller = ServiceLocator.Get<IUIManager>().GetController<BattleController>();
            
            // 隐藏行动提示
            controller.BattleUiManager.SetActTipActive(E_ActTipType.Hide);
            // 更新怪物血量UI
            controller.UiInitializer.InitMonsterUI(caster.Context.GetAliveMonsterEntitys());
            // 显示终结技立绘
            yield return controller.BattleUiManager.ShowPaiting((caster as PlayerObject)?.RoleInfo, skillInfo);

            // 获取终结技技能按键UI数据提供器
            var provider = ServiceLocator.Get<IFactoryManager>()
                .GetFactory<ISkillKeyUIDataProviderFactory, SkillKeyUIDataProviderFactory>()
                .GetCastSkillCondition<UltimateSkillKeyUIDataProvider>();
            
            // 根据数据更新玩家操作按键，按键触发技能选择事件
            controller.BattleUiManager.UpdateOperator(caster, provider);
        }
    }
}
