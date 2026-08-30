using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.StatSystem;
using HotUpdate.Game.Battle.StatSystem.Modifiers;
using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Status
{
    /// <summary>
    /// 渊涌
    /// </summary>
    [StatusTypeId(1051)]
    public class SurgingFlowStatus : StatusBase
    {
        protected long modifierId;
        
        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            ChangePine(-1);
        }
        
        protected override void OnAdd()
        {
            if (OwnerStatsComponent.TryGetModifier(EStatType.Atk, modifierId, out var modifier))
            {
                ((StatModifier)modifier).SetValue(50 * StatusProperty.CurrentPine);
            }
            else
            {
                var statModifier = modifierFactory.Create(EModifierType.Flat, 50 * StatusProperty.CurrentPine, out modifierId);
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
