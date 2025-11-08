using Framework;
using GameLogic.BattleMoudule.Entity;
using GameLogic.BattleMoudule.Event;

namespace GameLogic.BattleMoudule.Talent
{
    /// <summary>
    /// 希儿“再现”（击杀敌人后获得额外行动回合）
    /// </summary>
    public class ReappearanceTalent : ITalent
    {
        public string Name { get; } = "再现";

        public IBattleEntity Owner { get; }

        // 本回合是否已触发（防止多次触发）
        private bool _hasTriggeredThisTurn = false; 

        public ReappearanceTalent(IBattleEntity owner)
        {
            Owner = owner;
        }

        public bool CanTrigger(BattleEvent battleEvent, IBattleEntity owner)
        {
            if (battleEvent is not TurnEndEvent turnEndEvt)
            {
                return false;
            }

            // 触发条件：1. 是角色自身行动结束事件 2. 本回合击杀敌人(如何判断敌人是自己击败的) 3. 未触发过
            return turnEndEvt.CurrentCharacter == owner && turnEndEvt.HasKilledEnemy && !_hasTriggeredThisTurn;
        }

        public void Execute(BattleEvent battleEvent, IBattleEntity owner)
        {
            var turnEndEvt = (TurnEndEvent)battleEvent;
            LogMgr.Log($"\n【天赋触发】{owner.Name}触发天赋「{Name}」！");
            LogMgr.Log($"{owner.Name}获得额外行动回合！");

            // 核心逻辑：修改行动队列，将角色插入队首（调用核心层API，而非直接操作）(修改:应该是获得额外回合,而不是插入队首)
            turnEndEvt.Context.GetTurnManager().InsertToActionHead(owner);
            _hasTriggeredThisTurn = true; // 标记本回合已触发
        }

        public void OnTurnStartHandler(TurnStartEvent turnStartEvent)
        {
            if (turnStartEvent.CurrentCharacter == Owner)
            {
                _hasTriggeredThisTurn = false;
            }
        }

        public void OnTurnEndHandler(TurnEndEvent turnEndEvent)
        {
            /* 回合结束时不需要处理 */
        }
    }
}
