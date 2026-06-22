using System.Collections.Generic;
using HotUpdate.Game.Battle.Core;

namespace HotUpdate.Game.Battle.Summon
{
    public class SummonComponent : BattleComponent, ISummonComponent
    {
        private List<ISummon> _summons = new();

        public void CreateSummon<T>() where T : ISummon, new()
        {
            var summon = new T();
            summon.Init(BattleEntity);

            //typeof(T).GetProperty(nameof(ISummon.Owner)).SetValue(summon, _owner);
            //typeof(T).GetProperty("_initialActionTimes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(summon, initialActionTimes);

            _summons.Add(summon);
            //BattleEventBus.Publish(new SummonCreatedEvent(_owner.GetBattleComponent<IBattleContext>(), summon, _owner));
        }
        
        public List<ISummon> GetAllSummons() => _summons;
        
        protected override void OnBattleDestroy()
        {
            _summons.Clear();
            _summons = null;
        }
    }
}
