using System.ComponentModel;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Summon.Summons
{
    public class MimiSummon : BattleObject, ISummon
    {
        public IBattleEntityObject Owner { get; private set; }

        public void Init(IBattleEntityObject owner)
        {
            Owner = owner;

            //BattleEventBus.AddListener<SkillCastEvent>(OnOwnerSkillCastHandler);
        }

        public bool GetBattleComponent<TComponent>(out TComponent component) where TComponent : IComponent
        {
            bool isTrue = TryGetComponent<TComponent>(out TComponent c);
            component = c;
            return isTrue;
        }

        public override void CastSkill(int skillId)
        {
            
        }
    }
}
