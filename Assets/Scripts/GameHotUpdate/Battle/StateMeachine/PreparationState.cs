using Core.Config;
using Core.Service;
using Core.UI;
using Game.Battle.Context;
using Game.Battle.Enum;
using Game.Battle.Turn;
using Game.UI.Battle;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Battle.UI;
using GameHotUpdate.Battle.UI.Base;
using GameHotUpdate.Property;
using GameHotUpdate.UI.Loading.Battle;

namespace GameHotUpdate.Battle.StateMeachine
{
    public class PreparationState : BattleState
    {
        public PreparationState(IBattleStateMachine battleStateMachine, IBattleContext context) : base(battleStateMachine, context)
        {
            
        }

        public override void Enter()
        {
            Execute();
        }

        public override async void Execute()
        {
            // 创建战斗界面
            var battleController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<BattleView, BattleModel,BattleController>(E_UILayer.Mid, ResKeyCollection.BattleView);
            // 注册战斗界面UI调度器，依赖于战斗控制器
            ServiceLocator.Register<IBattleUIScheduler>(BattleUIScheduler.Instance); 
            // 显示战斗UI
            await battleController.InitBattleUI(Context);
            // 初始化行动顺序
            InitOrder();
            // 失活战斗界面
            ServiceLocator.Get<IUIManager>().SetViewActive(battleController, false);
            // 战斗准备完毕，销毁战斗加载界面
            var controller = ServiceLocator.Get<IUIManager>().GetController<BattleLoadingController>();
            ServiceLocator.Get<IUIManager>().DestroyView(controller);
            // 处理完毕
            BattleStateMachine.ChangeState(EBattlePhase.EnterAnimation);
        }
        
        /// <summary>
        /// 初始化顺序
        /// 用于选取第一个行动的实体
        /// </summary>
        public void InitOrder()
        {
            // 初始化所有角色的行动值
            foreach (var battleEntityObject in Context.GetAliveEntitys())
            {
                var speed = battleEntityObject.GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.CurrentSpeed);
                // 初始化行动值
                battleEntityObject.SetActionValue(CalcActionValue(speed));
            }

            // 基于行动值初始化行动顺序
            Context.Sort((b1, b2) =>
            {
                // 比较行动值确定行动顺序。行动值低，越先行动
                if (b1.ActionValue < b2.ActionValue)
                {
                    return -1;
                }

                return b1.ActionValue > b2.ActionValue ? 1 : 0;
            });

            // TODO：暂时这样处理：第一个行动的实体行动值为0，后续可能根据算法优化
            Context.GetFirstBattleEntity().SetActionValue(0);
            // 事件分发传递，更新行动轴UI显示
            Context.GetEventBus().TriggerEvent(new ActionBarSortPostEvent(Context, Context.GetAliveEntitys()));
        }

        public override void Exit()
        {

        }
    }
}
