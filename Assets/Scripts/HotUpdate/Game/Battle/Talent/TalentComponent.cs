using System.Collections.Generic;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Talent
{
    public class TalentComponent : BattleComponent, ITalentComponent
    {
        private List<ITalent> _talents = new();
        
        public void InitTalent(IBattleEntityObject battleEntity)
        {
            BattleInit(battleEntity);
            //BattleEntity.Context.GetEventBus().AddListener<TurnStartEvent>(OnBattleEventHandler);
            //BattleEntity.Context.GetEventBus().AddListener<TurnEndEvent>(OnBattleEventHandler);
        }
        
        private void OnBattleEventHandler(BattleEvent battleEvent)
        {
            // if (battleEvent is TurnStartEvent turnStartEvent)
            // {
            //     foreach (var talent in _talents)
            //     {
            //         //talent.OnTurnStartHandler(turnStartEvent);
            //     }
            // }

            // if (battleEvent is TurnEndEvent turnEndEvent)
            // {
            //     foreach (var talent in _talents)
            //     {
            //         talent.OnTurnEndHandler(turnEndEvent);
            //     }
            // }

            foreach (var talent in _talents)
            {
                if (talent.CanTrigger(battleEvent, BattleEntity))
                {
                    talent.Execute(battleEvent, BattleEntity);
                }
            }
        }

        public void AddTalent(ITalent talent)
        {
            _talents.Add(talent);
        }
        
        protected override void OnBattleDestroy()
        {
            _talents.Clear();
            _talents = null;
        }
    }
}
