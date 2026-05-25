using System.Collections;
using System.ComponentModel;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Summon.Summons
{
    /// <summary>
    /// �����ٻ���
    /// </summary>
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

        public override IEnumerator Die()
        {
            throw new System.NotImplementedException();
        }
    }
}
