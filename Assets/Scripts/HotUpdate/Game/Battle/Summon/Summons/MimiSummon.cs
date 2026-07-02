using System.ComponentModel;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.TargetSelect;

namespace HotUpdate.Game.Battle.Summon.Summons
{
    public class MimiSummon : BattleObject, ISummon
    {
        public IBattleEntityObject Owner { get; private set; }
        public override ISkillFactory SkillFactory { get; protected set; }
        public override ICastSkillCondition DefaultCastCondition { get; protected set; }
        public override ITargetSelectStrategy DefaultTargetSelectStrategy { get; protected set; }
        
        public void Init(IBattleEntityObject owner)
        {
            Owner = owner;

            //BattleEventBus.AddListener<SkillCastEvent>(OnOwnerSkillCastHandler);
        }

        public bool GetBattleComponent<TComponent>(out TComponent component) where TComponent : IComponent
        {
            var isTrue = TryGetComponent<TComponent>(out TComponent c);
            component = c;
            return isTrue;
        }

        protected override void OnExecuteAction()
        {
            
        }

        public override void CastSkill(int skillId)
        {
            
        }
    }
}
