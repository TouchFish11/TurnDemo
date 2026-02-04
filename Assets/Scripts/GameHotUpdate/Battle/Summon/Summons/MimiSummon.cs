using System.Collections;
using System.ComponentModel;
using Game.Battle.Objects;
using GameHotUpdate.Objects;

namespace GameHotUpdate.Battle.Summon.Summons
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
            // ���ġ����˼����ͷ��¼��������˷ż���ʱ���ٻ���Эͬ������(��ѡ)
            //BattleEventBus.AddListener<SkillCastEvent>(OnOwnerSkillCastHandler);
        }

        public bool GetBattleComponent<TComponent>(out TComponent component) where TComponent : IComponent
        {
            bool isTrue = TryGetComponent<TComponent>(out TComponent c);
            component = c;
            return isTrue;
        }

        public override void Heal(int value)
        {

        }

        protected override IEnumerator OnExceuteAction()
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerator Die()
        {
            throw new System.NotImplementedException();
        }
    }
}
