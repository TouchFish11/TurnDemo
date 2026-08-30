using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.StatSystem;
using HotUpdate.Game.Battle.StatSystem.Modifiers;
using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Status
{
    /// <summary>
    /// 恐惧III
    /// </summary>
    [StatusTypeId(221)]
    public class FearStatusIII : StatusBase
    {
        protected long modifierId;
        
        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            ChangePine(-1);
        }

        protected override void OnPineChanged()
        {
            if (OwnerStatsComponent.TryGetModifier(EStatType.Atk, modifierId, out var modifier))
            {
                ((StatModifier)modifier).SetValue(-40 * StatusProperty.CurrentPine);
            }
            else
            {
                var statModifier = modifierFactory.Create(EModifierType.Flat, -40 * StatusProperty.CurrentPine, out modifierId);
                OwnerStatsComponent.AddFinalStat(EStatType.Atk, statModifier);
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
