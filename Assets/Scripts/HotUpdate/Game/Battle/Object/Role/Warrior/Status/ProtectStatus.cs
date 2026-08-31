using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.StatSystem;
using HotUpdate.Game.Battle.StatSystem.Modifiers;
using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Status
{
    /// <summary>
    /// 庇佑
    /// </summary>
    [StatusTypeId(101)]
    public class ProtectStatus : StatusBase
    {
        protected long modifierId;
        
        protected override void OnAdd()
        {
            Owner.TakeSheild(150);
        }

        protected override void OnTurnEnd(IBattleEntityObject owner, IBattleContext context)
        {
            StatusProperty.RemainingRound -= 1;
        }

        protected override void OnPineChanged()
        {
            if (OwnerStatsComponent.TryGetModifier(EStatType.Def, modifierId, out var modifier))
            {
                ((StatModifier)modifier).SetValue(20 * StatusProperty.CurrentPine);
            }
            else
            {
                var statModifier = modifierFactory.Create(EModifierType.Flat, 20 * StatusProperty.CurrentPine, out modifierId);
                OwnerStatsComponent.AddFinalStat(EStatType.Def, statModifier);
            }
        }

        protected override void OnRemove()
        {
            if (OwnerStatsComponent.RemoveFinalStat(EStatType.Def, modifierId, out var statModifier))
            {
                modifierFactory.Release(statModifier);
            }
        }
    }
}
