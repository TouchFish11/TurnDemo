using System.Collections.Generic;
using System.Linq;
using HotUpdate.Game.Battle.Core;

namespace HotUpdate.Game.Battle.Relic
{
    public class RelicComponent : BattleComponent, IRelicComponent
    {
        private List<IRelic> _equippedRelics = new();
        private Dictionary<int, IRelicSetEffect> _activeSetEffects = new();

        public void EquipRelic(IRelic relic)
        {
            _equippedRelics.Add(relic);
            
            foreach (var effect in relic.SingleEffects)
            {
               // Caster.GetComponent<PropertyComponent>().AddRelicBonus(effect.RelicBoun, effect.BounValue);
            }
            
            CheckAndActivateSetEffects();
        }

        private void CheckAndActivateSetEffects()
        {
            
            var setCount = _equippedRelics.GroupBy(r => r.RelicID)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var (setId, count) in setCount)
            {
                var setEffect = RelicSetEffectFactory.Create(setId);
                if (setEffect == null || count < setEffect.RequiredCount) continue;
                
                setEffect.SetOwner(BattleEntity);
                setEffect.Activate(BattleEntity);
                _activeSetEffects.Add(setId, setEffect);
            }
        }

        protected override void OnBattleDestroy()
        {
            _equippedRelics.Clear();
            _equippedRelics = null;
            _activeSetEffects.Clear();
            _activeSetEffects = null;
        }
    }
}
