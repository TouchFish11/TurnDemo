using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.StatSystem;
using HotUpdate.Game.Battle.StatSystem.Modifiers;
using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Status
{
    /// <summary>
    /// 恐惧II
    /// </summary>
    [StatusTypeId(211)]
    public class FearStatusII : StatusBase
    {
        protected long modifierId;
        
        protected override void OnAdd()
        {
            var statModifier = modifierFactory.Create(EModifierType.Flat, -30 * StatusProperty.CurrentPine, out modifierId);
            OwnerStatsComponent.AddFinalStat(EStatType.Atk, statModifier);
        }
        
        protected override void OnTurnEnd(IBattleEntityObject owner, IBattleContext context)
        {
            StatusProperty.RemainingRound -= 1;
        }

        protected override void OnPineChanged()
        {
            if (OwnerStatsComponent.TryGetModifier(EStatType.Atk, modifierId, out var modifier))
            {
                ((StatModifier)modifier).SetValue(-30 * StatusProperty.CurrentPine);
            }
        }

        protected override void OnRemove()
        {
            if (OwnerStatsComponent.RemoveFinalStat(EStatType.Atk, modifierId, out var statModifier))
            {
                modifierFactory.Release(statModifier);
            }
        }
    }
}
